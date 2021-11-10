import React from "react"
import { ICodeDataView } from "../Services/ICodeDataView";
import { IServiceContainer } from "../Services/IServiceContainer";
import { IServiceProvider } from "../Services/IServiceProvider";
import { ServiceConstants } from "../Services/ServiceConstants";

type CodeDataViewProps = {
	services: IServiceProvider
}

type CodeDataViewState = {
	currentProcedureName: string|null
}

export class CodeDataView
	extends React.Component<CodeDataViewProps, CodeDataViewState>
	implements ICodeDataView
{
	constructor(props: CodeDataViewProps){
		super(props);
		this.state = {
			currentProcedureName: null
		}

		const sc = this.props.services.requireService<IServiceContainer>(ServiceConstants.IServiceContainer);
		sc.removeService(ServiceConstants.ICodeDataView);
		sc.addService(ServiceConstants.ICodeDataView, this);
	}

	showProcedure(procAddr: string): void {
		console.log(`showProcedure ${procAddr}`);
	}

	render(){
		return <></>;
	}
}