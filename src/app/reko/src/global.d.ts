// TS Definitions for the public methods exposed by the "back end".
interface RegisterEventListenerInterface { (eventName: string, eventHandler: Function) }
interface DisassembleRandomBytesInterface { (address: string, otherArg: string) }
interface OpenFileInterface { (): string }
interface LoadFileInterface { (filePath: string, loader: string? = null) : boolean; }
interface RenderProjectViewInterface { (): string }
interface ScanImagesInterface { ():any }
// $TODO: should return a JSON Array
interface DumpBytesInterface { (programName: string, address: string, length: number): string }
interface GetProcedureListInterface { (filter:string):any}

type RekoObject = {
	RegisterEventListener: RegisterEventListenerInterface,
	Proto_DisassembleRandomBytes: DisassembleRandomBytesInterface,
	OpenFile: OpenFileInterface,
	LoadFile: LoadFileInterface,
	RenderProjectView: RenderProjectViewInterface,
	DumpBytes: DumpBytesInterface,
	Scan: ScanImagesInterface,
	GetProcedureList: GetProcedureListInterface,
}

interface Window {
	reko: RekoObject
}