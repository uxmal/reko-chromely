import React from 'react';

export type DiagnosticsListenerProps = {
	onMessage: (msg:string) => void;
}

export class DiagnosticsListener {
	public static register(sink:string, onMessage: (msg:string) => void){
		window.reko.RegisterEventListener("Listener.Info", onMessage);
	}
}