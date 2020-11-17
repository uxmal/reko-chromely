import React from "react";

export type RekoMenuProps = {
	onDisassembleBytes: () => void,
	onClearDasm: () => void,
	onOpenFile: () => void,
	onLoadImage: () => void
}

export type MenuItemProps = {
	id: number,
	text: string,
	icon: string,
}

export type MenuItemState = {
	enabled: boolean,
	visible: boolean
}

export class MenuItem extends React.Component<MenuItemProps,MenuItemState>{

	constructor(props:MenuItemProps){
		super(props);
	}

	render(){
		return <div>
			{this.props.children}
		</div>;
	}
}

export class RekoMenu extends React.Component<RekoMenuProps,{}>{
	constructor(props:RekoMenuProps){
		super(props);
	}

	render(){
		return <div>
			<button type="button" onClick={this.props.onOpenFile.bind(this)}>Open File</button>
			<button type="button" onClick={this.props.onLoadImage.bind(this)}>Load Image</button>
			<button type="button" onClick={this.props.onDisassembleBytes.bind(this)}>Disassemble some random X86 code</button>
			<button type="button" onClick={this.props.onClearDasm.bind(this)}>Clear disassembly</button>
		</div>
	}
}