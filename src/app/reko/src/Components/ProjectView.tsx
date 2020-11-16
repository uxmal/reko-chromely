import React from "react";

export type ProjectViewProps = {
	content: string
}

export class ProjectView extends React.Component<ProjectViewProps,{}>{
	constructor(props:ProjectViewProps){
		super(props);
	}

	render(){
		return <div dangerouslySetInnerHTML={{__html: this.props.content }}></div>
	}
}