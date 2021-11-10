import React, { LegacyRef, ReactHTML, RefObject } from 'react';
import { AutoSizer, CellMeasurer, CellMeasurerCache, Grid, GridCellProps } from 'react-virtualized';
import { CellMeasurerChildProps } from 'react-virtualized/dist/es/CellMeasurer';
import { IServiceProvider } from '../Services/IServiceProvider';

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
    filter: string,
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
                data-addr={proc.sAddress}>{proc.name}
        </div>
    }
}

export type ProcedureListProps = {
    services: IServiceProvider
    scanned: boolean
}

export class ProcedureList extends React.Component<ProcedureListProps, ProcedureListState>
{
    private readonly cache = new CellMeasurerCache({
        minWidth: 75,
        defaultWidth: 200,
        fixedHeight: false,
        fixedWidth: false
    });

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

    private renderProcedure(props: GridCellProps){
        var proc = this.state.procs[props.rowIndex];

        return <CellMeasurer
            cache={this.cache}
            columnIndex={props.columnIndex}
            key={props.key}
            parent={props.parent}
            rowIndex={props.rowIndex}
        >
            {
            (childProps: CellMeasurerChildProps) => (
                    <div
                        ref={childProps.registerChild as any}
                        style={props.style}
                    >
                        <Procedure
                            key={proc.sAddress}
                            procData={proc}
                            filter={this.state.filter}></Procedure>
                    </div>
                )
            }
        </CellMeasurer>
    }

    onClick(e: React.UIEvent<HTMLElement>){
        const procNode = e.target as HTMLDivElement
        if(!procNode) return
        e.preventDefault();
        
        const procAddr = procNode.getAttribute("data-addr");
    }

    render() {
        if (!this.state.procsFetched)
        {
            return <div>No procedures available.</div>;
        }

        return <div
                className={"grid-container"}
                onClick={this.onClick.bind(this)}
            >
            <Grid 
                columnWidth={this.cache.columnWidth}
                rowHeight={this.cache.rowHeight}
                deferredMeasurementCache={this.cache}
                cellRenderer={this.renderProcedure.bind(this)}
                rowCount={this.state.procs.length}
                columnCount={1}
                height={400}
                width={400}
                autoHeight={true}
                autoWidth={true}
                autoContainerWidth={true}
            />
        </div>
    }
}