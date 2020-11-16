import React from "react";

export class DiagnosticsArea extends React.Component<{},{}>{
	constructor(props:any){
		super(props);
	}

	render(){
		return <div>
			{this.props.children}
		</div>
	}
}