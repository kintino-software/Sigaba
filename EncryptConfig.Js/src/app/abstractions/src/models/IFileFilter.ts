export interface IFileFilter {
	match(filePath: string): boolean;
}
