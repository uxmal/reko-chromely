import { IServiceContainer } from './IServiceContainer';
import { IServiceProvider } from './IServiceProvider';

type ServiceDict = { [key: string] : object }

export class ServiceContainer implements IServiceProvider, IServiceContainer {	
	private readonly services:ServiceDict = {}

	public constructor(){
		this.addService("IServiceContainer", this);
	}

	public addService(serviceId: string, serviceInstance: object): void {
		const svc = this.getService(serviceId);
		if(svc){
			throw new Error(`Service ${serviceId} already exists`);
		}
		this.services[serviceId] = serviceInstance;
	}

	public requireService(serviceId: string): object {
		const svc = this.getService(serviceId);
		if(!svc){
			throw new Error(`Unknown service ${serviceId}`);
		}
		return svc;
	}

	public getService(serviceId: string) {
		return this.services[serviceId] ?? null;
	}
}