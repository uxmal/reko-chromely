import React from 'react';
import { DiagnosticsListener } from './Services/DiagnosticsListener';
import { DiagnosticMessage } from './Components/DiagnosticMessage';
import { RekoMenu } from './Components/RekoMenu';
import { RekoDisassemblyView } from './Components/RekoDisassemblyView';
import { DiagnosticsArea } from './Components/DiagnosticsArea';
import { ImageMapComponent } from './Components/ImageMapComponent';

import './style.css';
import { ProjectView } from './Components/ProjectView';
import { BytesDumpView } from './Components/BytesDumpView';

type AppState = {
	diagnosticMessages: JSX.Element[],
	dasmContent: string,
	projectViewContent: string,
	filePath: string|null,
	loaded: boolean
};

class App extends React.Component<{},AppState> {

	constructor(props:any){
		super(props);
		this.state = {
			diagnosticMessages: [],
			dasmContent: "",
			projectViewContent: "",
			filePath: null,
			loaded: false
		};
	}

	componentDidMount(){
		new DiagnosticsListener({
			onMessage: this.onDiagnosticMessage.bind(this)
		});
		
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
		if(this.state.filePath == null){
			return;
		}
		let result = await window.reko.LoadFile(this.state.filePath, null);
		if(!result){
			return;
		}

		
		let projectViewContent = await window.reko.RenderProjectView();
		this.setState({
			projectViewContent: projectViewContent
		});



		console.log("OK");
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
				/>

				<span>Current File: {this.state.filePath}</span>

				<br />
				<ImageMapComponent />

				<RekoDisassemblyView content={this.state.dasmContent} />
				<ProjectView content={this.state.projectViewContent} />

				<BytesDumpView
					programName={App.basename(this.state.filePath ?? "")}
					length={16}
				/>

				<DiagnosticsArea>
					{this.state.diagnosticMessages}
				</DiagnosticsArea>
		</div>
	}
}

export default App;
