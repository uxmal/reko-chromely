import React from 'react';
import { DiagnosticsListener } from './Services/DiagnosticsListener';
import { DiagnosticMessage } from './Components/DiagnosticMessage';
import { DocumentWindowFrame } from './Components/DocumentWindowFrame';
import { RekoMenu } from './Components/RekoMenu';
import { RekoDisassemblyView } from './Components/RekoDisassemblyView';
import { DiagnosticsArea } from './Components/DiagnosticsArea';
import { ImageMapComponent } from './Components/ImageMapComponent';
import { ProcedureList } from './Components/ProcedureList';
import { ToolWindowFrame } from './Components/ToolWindowFrame';

import './css/index.css';
import './css/reko.css';

import { ProjectView } from './Components/ProjectView';
import { BytesDumpView } from './Components/BytesDumpView';

type AppState = {
	diagnosticMessages: JSX.Element[],
	dasmContent: string,
	projectViewContent: string,
	filePath: string|null,
	loaded: boolean,
	scanned: boolean
};

class App extends React.Component<{},AppState> {

	constructor(props:any){
		super(props);
		this.state = {
			diagnosticMessages: [],
			dasmContent: "",
			projectViewContent: "",
			filePath: null,
			loaded: false,
			scanned: false
		};
	}

	componentDidMount(){
		DiagnosticsListener.register("Listener.Info", this.onDiagnosticMessage.bind(this));
		let awindow = window as any;
		awindow.reko.TestListener();
	}

	private onDiagnosticMessage(msg:string){
		let messages = this.state.diagnosticMessages;

		let msgElem = <DiagnosticMessage key={messages.length} message={msg} />;
		messages.push(msgElem);
		
		this.setState({
			diagnosticMessages: messages
		});
	}

	// User asked to disassemble some instructions.
	private async onDisassembleBytes(){
		let content = await window.reko.Proto_DisassembleRandomBytes("0010000", "00010");
		this.setState({
			dasmContent: content
		});
	}

	private async onClearDasm(){
		this.setState({
			dasmContent: ""
		});
	}

	private async onOpenFile(){
		let filePath = await window.reko.OpenFile();
		this.setState({
			filePath: filePath
		});
	}

	private async onLoadImage(){
		this.setState({
			loaded: false,
			scanned: false
		});

		if(this.state.filePath == null){
			return;
		}
		let result = await window.reko.LoadFile(this.state.filePath, null, null);
		if(!result){
			return;
		}

		
		let projectViewContent = await window.reko.RenderProjectView();
		this.setState({
			projectViewContent: projectViewContent,
			loaded: true
		});

		console.log("File loaded.");
	}

	private async onScanImage() {
		if(!this.state.loaded){
			return;
		}

		console.log("Scanning...");
		await window.reko.Scan();
		console.log("Scanning...Done");
		this.setState({
			scanned: true
		});
	}

	private static basename(str:string):string {
		let normalized = str.replace(/\\/g, "/");
		let base = new String(normalized).substring(normalized.lastIndexOf('/') + 1);
		return base;
	}

	render(){
		return <div>
				<RekoMenu
					onOpenFile={this.onOpenFile.bind(this)}
					onLoadImage={this.onLoadImage.bind(this)}
					onDisassembleBytes={this.onDisassembleBytes.bind(this)}
					onClearDasm={this.onClearDasm.bind(this)}
					onScanImage={this.onScanImage.bind(this)}
				/>

				<span>Current File: {this.state.filePath}</span>

				<br />
				<ImageMapComponent />
				<table className="mainView"><tbody>
				<tr>
					<td id="projectBrowser">
						<ToolWindowFrame title="Project browser">
							<ProjectView content={this.state.projectViewContent} />
						</ToolWindowFrame>
					</td>
					<td id="procedureList">
						<ToolWindowFrame title="Procedure List">
							<ProcedureList scanned={this.state.scanned} />
						</ToolWindowFrame>
					</td>
					<td className="documentArea">
						<DocumentWindowFrame title="Memory">
							<BytesDumpView
								programName={App.basename(this.state.filePath ?? "")}
								length={16}
							/>
						</DocumentWindowFrame>

						<DocumentWindowFrame title="Disassembly">
							<RekoDisassemblyView content={this.state.dasmContent} />
						</DocumentWindowFrame>
					</td>
				</tr>

				</tbody></table>

				<DocumentWindowFrame title="Diagnostics">
					<DiagnosticsArea>{this.state.diagnosticMessages}</DiagnosticsArea>
				</DocumentWindowFrame>
		</div>
	}
}

export default App;
