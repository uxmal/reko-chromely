import React from 'react';

type ToolWindowFrameProperties = 
{
    title: string;
}

export class ToolWindowFrame extends React.Component<ToolWindowFrameProperties,{}>
{
    constructor (props:ToolWindowFrameProperties)
    {
        super(props);
    }

    render() {
        return <div className="toolWindow">
        <div className="caption">{this.props.title}</div>
        {this.props.children}
    </div>;

    }
}
