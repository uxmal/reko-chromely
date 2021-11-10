import { IServiceContainer } from './IServiceContainer';
import { IServiceProvider } from './IServiceProvider';
import { ServiceConstants } from './ServiceConstants';

type ServiceDict = { [key: string] : object }

export class ServiceContainer implements IServiceProvider, IServiceContainer {	
	private readonly services:ServiceDict = {}

	public constructor(){
		this.addService(ServiceConstants.IServiceContainer, this);
	}

	public removeService<T extends object>(serviceId: string): boolean {
		const svc = this.getService<T>(serviceId)
		if(svc){
			delete this.services[serviceId];
			return true;
		}
		return false;
	}

	public addService<T extends object>(serviceId: string, serviceInstance: T): void {
		const svc = this.getService(serviceId);
		if(svc){
			throw new Error(`Service ${serviceId} already exists`);
		}
		this.services[serviceId] = serviceInstance;
	}

	public requireService<T extends object>(serviceId: string): T {
		const svc = this.getService(serviceId);
		if(!svc){
			throw new Error(`Unknown service ${serviceId}`);
		}
		return svc as T;
	}

	public getService<T extends object>(serviceId: string) {
		const obj = this.services[serviceId];
		if(!obj) return null;
		return obj as T;
	}
}