import React from "react";

export type RekoDisassemblyViewProps = {
	content: string
};

export class RekoDisassemblyView extends React.Component<RekoDisassemblyViewProps, {}> {
	constructor(props:RekoDisassemblyViewProps){
		super(props);
		this.state = {
			content: ""
		}
	}

	render(){
		return <div dangerouslySetInnerHTML={{ __html: this.props.content}}></div>
	}
}