// TS Definitions for the public methods exposed by the "back end".
interface RegisterEventListenerInterface { (eventName: string, eventHandler: Function) }
interface DisassembleRandomBytesInterface { (address: string, otherArg: string) }
interface OpenFileInterface { (): string }
// $TODO: address is not serializable
interface LoadFileInterface { (filePath: string, loader: string? = null, address: object? = null) : boolean; }
interface RenderProjectViewInterface { (): string }
interface ScanImagesInterface { ():any }
// $TODO: should return a JSON Array
interface DumpBytesInterface { (programName: string, address: string, length: number): string }
interface GetProcedureListInterface { (filter:string):any}
interface RenderProcedureInterface { (key:number):object }

type RekoObject = {
	RegisterEventListener: RegisterEventListenerInterface,
	Proto_DisassembleRandomBytes: DisassembleRandomBytesInterface,
	OpenFile: OpenFileInterface,
	LoadFile: LoadFileInterface,
	RenderProjectView: RenderProjectViewInterface,
	DumpBytes: DumpBytesInterface,
	Scan: ScanImagesInterface,
	GetProcedureList: GetProcedureListInterface,
	RenderProcedure: RenderProcedureInterface
}

interface Window {
	reko: RekoObject
}