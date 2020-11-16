import React from "react";

type DiagnosticMessageProps = {
	message: string
}

export class DiagnosticMessage extends React.Component<DiagnosticMessageProps,{}> {
	constructor(props:DiagnosticMessageProps){
		super(props);
	}

	render(){
		return <div dangerouslySetInnerHTML={{ __html: this.props.message}}></div>
	}
}