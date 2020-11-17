interface RegisterEventListenerInterface { (eventName: string, eventHandler: Function) }
interface DisassembleRandomBytesInterface { (address: string, otherArg: string) }
interface OpenFileInterface { (): string }
interface LoadFileInterface { (filePath: string, loader: string? = null) : boolean; }
interface RenderProjectViewInterface { (): string }
interface ScanImagesInterface { ():any }
// $TODO: should return a JSON Array
interface DumpBytesInterface { (programName: string, address: string, length: number): string }

type RekoObject = {
	RegisterEventListener: RegisterEventListenerInterface,
	Proto_DisassembleRandomBytes: DisassembleRandomBytesInterface,
	OpenFile: OpenFileInterface,
	LoadFile: LoadFileInterface,
	RenderProjectView: RenderProjectViewInterface,
	DumpBytes: DumpBytesInterface,
	Scan: ScanImagesInterface
}

interface Window {
	reko: RekoObject
}