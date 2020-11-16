import React from 'react';

export type DiagnosticsListenerProps = {
	onMessage: (msg:string) => void;
}

export class DiagnosticsListener {
	constructor(private readonly props:DiagnosticsListenerProps){
		window.reko.RegisterEventListener("Listener.Info", this.props.onMessage);
	}
}