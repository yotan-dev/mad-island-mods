import * as fs from 'fs';
import archiver from 'archiver';
import assert from 'assert';
import { execSync } from 'child_process';
import { mkdirSync, cpSync, existsSync, rmSync, readFileSync } from 'fs';
import { dirname, join, resolve } from 'path';

const rootDir = resolve('..');
const artifactsDir = join(rootDir, 'Release');
const config = 'Release';

function runCommand(command, cwd = rootDir) {
	console.log(`Running: ${command}`);
	try {
		execSync(command, { stdio: 'inherit', cwd, shell: true });
	} catch {
		console.error(`Command failed: ${command}`);
		process.exit(1);
	}
}

function cleanArtifacts() {
	console.log('Cleaning artifacts directory...');
	if (existsSync(artifactsDir)) {
		rmSync(artifactsDir, { recursive: true, force: true });
	}
	mkdirSync(artifactsDir, { recursive: true });
}

/**
 *
 * @param {Array<{ from: string, to: string }>} files
 */
function copyToRelease(files) {
	console.log('Copying files to release...');
	for (const file of files) {
		const toDir = resolve(artifactsDir, dirname(file.to));
		mkdirSync(toDir, { recursive: true });
		cpSync(file.from, resolve(artifactsDir, file.to), { recursive: true, force: true });
	}
}

function getBinaryPath(projectPath) {
	return resolve(projectPath, 'bin', config, 'netstandard2.1');
}

function getProjectVersion(csprojPath) {
	const content = readFileSync(csprojPath, 'utf-8');
	const versionMatch = content.match(/<Version>(.*?)<\/Version>/);
	assert(versionMatch[1], `Version not found in ${csprojPath}`);
	return versionMatch[1];
}

async function pack(name, version, inputDir) {
	const to = resolve(artifactsDir, `${name}-${version}.zip`);
	return new Promise((res, rej) => {
		const output = fs.createWriteStream(to);
		const archive = archiver('zip', {
			zlib: { level: 9 }, // Sets the compression level.
		});
		output.on('close', () => {
			console.log(`✅ ${to} (${archive.pointer()} bytes)`);
			res();
		});
		archive.on('error', err => rej(err));
		archive.pipe(output);
		archive.directory(inputDir, false);
		archive.finalize();
	});
}

async function buildYotanModCore() {
	console.log('=== Building YotanModCore ===');

	const corePath = join(rootDir, 'YotanModCore');
	const loaderPath = join(rootDir, 'YotanModCoreLoader');
	const patcherPath = join(rootDir, 'YotanModCorePatcher');

	runCommand('dotnet restore', corePath);
	runCommand(`dotnet build -c ${config}`, corePath);
	runCommand('dotnet restore', loaderPath);
	runCommand(`dotnet build -c ${config}`, loaderPath);
	runCommand('dotnet restore', patcherPath);
	runCommand(`dotnet build -c ${config}`, patcherPath);

	copyToRelease([
		{
			from: resolve(getBinaryPath(loaderPath), 'YotanModCore.dll'),
			to: 'YotanModCore/BepInEx/plugins/YotanModCore/YotanModCore.dll',
		},
		{
			from: resolve(getBinaryPath(loaderPath), 'YotanModCoreLoader.dll'),
			to: 'YotanModCore/BepInEx/plugins/YotanModCore/YotanModCoreLoader.dll',
		},
		{
			from: resolve(loaderPath, 'YotanModCore.assets'),
			to: 'YotanModCore/BepInEx/plugins/YotanModCore/YotanModCore.assets',
		},
		{
			from: resolve(getBinaryPath(patcherPath), 'YotanModCorePatcher.dll'),
			to: 'YotanModCore/BepInEx/patchers/YotanModCorePatcher.dll',
		}
	]);

	const version = getProjectVersion(resolve(loaderPath, 'YotanModCoreLoader.csproj'));

	console.log('Done.');

	return version;
}

async function buildHFramework() {
	console.log('=== Building HFramework ===');

	const libPath = join(rootDir, 'HFramework');

	runCommand('dotnet restore', libPath);
	runCommand(`dotnet build -c ${config}`, libPath);

	copyToRelease([
		{
			from: resolve(getBinaryPath(libPath), 'HFramework.dll'),
			to: 'HFramework/HFramework/HFramework.dll',
		},
		{
			from: resolve(libPath, 'definitions'),
			to: 'HFramework/HFramework/definitions',
		}
	]);

	const version = getProjectVersion(resolve(libPath, 'HFramework.csproj'));

	console.log('Done.');

	return version;
}

async function buildEnhancedIsland() {
	console.log('=== Building EnhancedIsland ===');

	const libPath = join(rootDir, 'EnhancedIsland');

	runCommand('dotnet restore', libPath);
	runCommand(`dotnet build -c ${config}`, libPath);

	copyToRelease([
		{
			from: resolve(getBinaryPath(libPath), 'EnhancedIsland.dll'),
			to: 'EnhancedIsland/EnhancedIsland/EnhancedIsland.dll',
		},
		{
			from: resolve(libPath, 'assets/DisassembleTable.json'),
			to: 'EnhancedIsland/EnhancedIsland/DisassembleTable.json',
		},
	]);

	const version = getProjectVersion(resolve(libPath, 'EnhancedIsland.csproj'));

	console.log('Done.');

	return version;
}

async function buildGallery() {
	console.log('=== Building Gallery ===');

	const libPath = join(rootDir, 'Gallery');

	runCommand('dotnet restore', libPath);
	runCommand(`dotnet build -c ${config}`, libPath);

	copyToRelease([
		{
			from: resolve(getBinaryPath(libPath), 'Gallery.dll'),
			to: 'Gallery/Gallery/Gallery.dll',
		},
		{
			from: resolve(libPath, 'GalleryList.xml'),
			to: 'Gallery/Gallery/GalleryList.xml',
		},
		{
			from: resolve(libPath, 'GalleryAssets.assets'),
			to: 'Gallery/Gallery/GalleryAssets.assets',
		}
	]);

	const version = getProjectVersion(resolve(libPath, 'Gallery.csproj'));

	console.log('Done.');

	return version;
}

async function buildHExtensions() {
	console.log('=== Building HExtensions ===');

	const libPath = join(rootDir, 'HExtensions');

	runCommand('dotnet restore', libPath);
	runCommand(`dotnet build -c ${config}`, libPath);

	copyToRelease([
		{
			from: resolve(getBinaryPath(libPath), 'HExtensions.dll'),
			to: 'HExtensions/HExtensions/HExtensions.dll',
		},
		{
			from: resolve(libPath, 'definitions'),
			to: 'HExtensions/HExtensions/definitions',
		}
	]);

	const version = getProjectVersion(resolve(libPath, 'HExtensions.csproj'));

	console.log('Done.');

	return version;
}

async function buildYoUnnoficialPathces() {
	console.log('=== Building YoUnnoficialPatches ===');

	const libPath = join(rootDir, 'YoUnnoficialPatches');

	runCommand('dotnet restore', libPath);
	runCommand(`dotnet build -c ${config}`, libPath);

	copyToRelease([
		{
			from: resolve(getBinaryPath(libPath), 'YoUnnoficialPatches.dll'),
			to: 'YoUnnoficialPatches/YoUnnoficialPatches.dll',
		},
	]);

	const version = getProjectVersion(resolve(libPath, 'YoUnnoficialPatches.csproj'));

	console.log('Done.');

	return version;
}



async function main() {
	console.log('Starting build process...');

	cleanArtifacts();

	// First build the base frameworks.
	const ymcVersion = await buildYotanModCore();
	await pack('YotanModCore', ymcVersion, resolve(artifactsDir, 'YotanModCore'));

	const hfVersion = await buildHFramework();
	await pack('HFramework', hfVersion, resolve(artifactsDir, 'HFramework'));

	// Now build leaf mods
	await Promise.all([
		(async () => {
			const version = await buildEnhancedIsland();
			await pack('EnhancedIsland', version, resolve(artifactsDir, 'EnhancedIsland'));
		})(),
		(async () => {
			const version = await buildGallery();
			await pack('Gallery', version, resolve(artifactsDir, 'Gallery'));
		})(),
		(async () => {
			const version = await buildHExtensions();
			await pack('HExtensions', version, resolve(artifactsDir, 'HExtensions'));
		})(),
		(async () => {
			const version = await buildYoUnnoficialPathces();
			await pack('YoUnnoficialPatches', version, resolve(artifactsDir, 'YoUnnoficialPatches'));
		})(),
	]);

	console.log('\nBuild completed successfully!');
	console.log(`Artifacts available in: ${artifactsDir}`);
}

// Run the build process
main().catch(error => {
	console.error('Build failed:', error);
	process.exit(1);
});
