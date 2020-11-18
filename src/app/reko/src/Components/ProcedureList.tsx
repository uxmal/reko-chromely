import React, { ReactHTML } from 'react';

type ProcedureListItem = {
    sProgram : string;
    sAddress : string;
    name: string;
}

type ProcedureListState = {
    procs : ProcedureListItem[]
}

export class ProcedureList extends React.Component<{}, ProcedureListState>
{
    public constructor(props:any)
    {
        super(props);
        this.state = {
            procs: []
        };
    }

    onFilterTextChanged(e: React.ChangeEvent<HTMLInputElement>) {
        this.fetchProcedureList(e.target.value);
    }

    async fetchProcedureList(filter: string) {
        console.log("ProcedureList: fetching procedure list...");
        let jsProcs = await window.reko.GetProcedureList(filter);
        let procs = JSON.parse(jsProcs);
        console.log("ProcedureList: received " + procs.length + " procedures");
        this.setState({
            procs: procs
        });
    }

    render() {
        if (this.state === null || this.state.procs === null)
        {
            return <div>No procedures available.</div>;
        }
        else
        {
            let procs = this.state.procs.map(pitem =>
            {
                return <div 
                    className="item"
                    key={pitem.sAddress}
                    data-addr={pitem.sAddress}>{pitem.name}</div>
            });
            return <div className="procedureList">
                <input type="text" onChange={this.onFilterTextChanged.bind(this)} />
                {procs}
            </div>
        }
    }
}