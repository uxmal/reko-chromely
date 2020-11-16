import React from "react";

export class ImageMapComponent extends React.Component<{},{}>{
	constructor(props:any){
		super(props);
	}

	render(){
		return <>
			<img src="reko://GeneratePng?percent=60" />
		</>;
	}
}