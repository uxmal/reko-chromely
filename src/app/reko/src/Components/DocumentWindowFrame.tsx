import React from "react";

type DocumentWindowFrameProps={
    title:string
}

export class DocumentWindowFrame extends React.Component<DocumentWindowFrameProps, {}>
{
    constructor(props: any) {
        super(props);
    }

    render() {
        return <div className="toolWindow">
            <div className="caption">{this.props.title}</div>
            {this.props.children}
        </div>;
    }
}