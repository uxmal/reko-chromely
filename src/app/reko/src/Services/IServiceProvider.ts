export interface IServiceProvider {
	requireService<T extends object>(serviceId: string): T
	getService<T extends object>(serviceId: string): T|null
}