import React from "react";

type DiagnosticMessageProps = {
	message: string
}

export class DiagnosticMessage extends React.Component<DiagnosticMessageProps,{}> {

	private div:React.RefObject<HTMLDivElement>;

	constructor(props:DiagnosticMessageProps){
		super(props);
		this.div = React.createRef<HTMLDivElement>();
	}

	onRender(){
		let items = this.div.current?.getElementsByClassName("diag-inf")!;
		items.item(0)?.addEventListener("click", () => {
			alert("foo");
		});
	}

	componentDidMount(){
		this.onRender();
	}

	componentDidUpdate(){
		this.onRender();
	}

	render(){
		return <div ref={this.div} dangerouslySetInnerHTML={{ __html: this.props.message}}></div>
	}
}