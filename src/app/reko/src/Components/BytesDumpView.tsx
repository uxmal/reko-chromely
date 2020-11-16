import React from "react";

type BytesDumpViewProps = {
	programName: string,
	length: number
}

type BytesDumpItem = {
	t: string,
	d: string
}

type BytesDumpLine = {
	addr: string,
	addrLabel: string,
	hex: BytesDumpItem[];
}

type BytesDumpResult = BytesDumpLine[];

type BytesDumpViewState = {
	data: BytesDumpResult
}

export class BytesDumpView extends React.Component<BytesDumpViewProps,BytesDumpViewState>{

	async fetchData(address:string){
		//$TODO: should return JSON already!
		console.log(address);
		let result = await window.reko.DumpBytes(this.props.programName, address, this.props.length);
		console.log(result);

		this.setState({
			data: JSON.parse(result)
		});
	}

	constructor(props:BytesDumpViewProps){
		super(props);
		this.state = {
			data: []
		};
	}

	onDumpAddressChange(e:React.ChangeEvent<HTMLInputElement>){
		this.fetchData(e.target.value);
	}

	render(){
		let data = this.state.data;

		let rows = data.map(r => {
			let items = r.hex.map((b,i) => {
				return <span key={i}>{b.d}</span>;
			});

			return <div key={r.addr}>
				<span>{r.addrLabel}</span>
				<div>
					{items}
				</div>
			</div>;
		});

		let outer = <div>
			<input type="text" onChange={this.onDumpAddressChange.bind(this)}></input>
			{rows}
		</div>;

		return outer;
	}
	
}