import React, { ReactHTML } from 'react';

type ProcedureListItem = {
    sProgram : string;
    sAddress : string;
    name: string;
}

type ProcedureListState = {
    procs : ProcedureListItem[],
    filter: string,
    procsFetched: boolean
}

type ProcedureProps = {
    procData: ProcedureListItem,
    filter: string
}

export class Procedure extends React.Component<ProcedureProps, {}>{
    private ref:React.RefObject<HTMLDivElement>;

    public constructor(props:ProcedureProps){
        super(props);
        this.ref = React.createRef();
    }

    render(){
        let proc = this.props.procData;
        return <div 
                ref={this.ref}
                className="item"
                key={proc.sAddress}
                style={{
                    display: (proc.name.includes(this.props.filter)) ? 'inherit' : 'none'
                }}
                data-addr={proc.sAddress}>{proc.name}</div>
    }
}

export type ProcedureListProps = {
    scanned: boolean
}

export class ProcedureList extends React.Component<ProcedureListProps, ProcedureListState>
{
    public constructor(props:any)
    {
        super(props);
        this.state = {
            procs: [],
            filter: '',
            procsFetched: false
        };
    }

    onFilterTextChanged(e: React.ChangeEvent<HTMLInputElement>) {
        this.setState({
            filter: e.target.value
        });
    }

    async componentDidMount(){
        let procs = await this.fetchProcedureList();
        this.setState({
            procs: procs
        });
    }

    async fetchProcedureList() {
        console.log("ProcedureList: fetching procedure list...");
        let jsProcs = await window.reko.GetProcedureList('');
        let newProcs = JSON.parse(jsProcs);
        console.log(`ProcedureList: received ${newProcs.length} procedures`);
        return newProcs;
    }

    async componentDidUpdate(){
        if(this.props.scanned){
            if(!this.state.procsFetched){
                // scanned, but not fetched. go fetch 'em
                let procs = await this.fetchProcedureList();
                this.setState({
                    procs: procs,
                    procsFetched: true
                });
            }
        } else if(this.state.procsFetched) {
            // not scanned, and we had fetched procedures
            // reset state to fetch procedures again
            this.setState({
                procs: [],
                procsFetched: false,
            });
        }
    }

    render() {
        if (!this.state.procsFetched)
        {
            return <div>No procedures available.</div>;
        }

        let procs = this.state.procs.map(pitem =>
        {
            return <Procedure key={pitem.sAddress} procData={pitem} filter={this.state.filter} />
        });
        return <div className="procedureList">
            <input type="text" onChange={this.onFilterTextChanged.bind(this)} />
            {procs}
        </div>
    }
}