import chokidar from 'chokidar';
import fs from 'fs';
import json5 from 'json5';

function expandAliases(conf) {
	let pendingExpansion = true;
	let loopCount = 0;
	while (pendingExpansion && loopCount < 50) {
		loopCount++;

		for (const [aliasKey, aliasValue] of Object.entries(conf.aliases)) {
			for (const target of Object.values(conf.target)) {
				target.from = target.from.replace(`%${aliasKey}%`, aliasValue);
				target.to = target.to.map(p => p.replace(`%${aliasKey}%`, aliasValue));
			}
		}

		pendingExpansion = false;
		for (const target of Object.values(conf.target)) {
			if (target.from.includes('%')) {
				pendingExpansion = true;
				break;
			}
			for (const toPath of target.to) {
				if (toPath.includes('%')) {
					pendingExpansion = true;
					break;
				}
			}
		}
	}

	if (loopCount >= 50) {
		console.log(JSON.stringify(conf.target, null, 2));
		throw new Error('Could not expand aliases after 50 loops');
	}

	return conf;
}

const config = expandAliases(json5.parse(fs.readFileSync('./watcherConfig.json', 'utf-8')));

const watchReplace = config.target;

console.log('----- Initializing -------');
console.log('Expanded config:');
console.log(watchReplace);
console.log('--------------------------');

// Initialize watcher
const watchPaths = [...new Set(watchReplace.map(w => w.from))];
const watcher = chokidar.watch(watchPaths);

// Something to use when events are received.
const log = console.log.bind(console);
// Add event listeners.
watcher
	.on('add', (path) => {
		path = path.replace(/\\/g, '/');
		log(`File ${path} has been added`);
		const to = watchReplace.find(w => w.from === path)?.to;
		if (to) {
			to.forEach((toPath) => {
				log(`Copying ${path} with ${toPath}`);
				fs.copyFileSync(path, toPath);
			});
		} else {
			log(`Ignoring add of ${path}`);
		}
	})
	.on('change', (path) => {
		path = path.replace(/\\/g, '/');
		log(`File ${path} has been changed`);
		const to = watchReplace.find(w => w.from === path)?.to;
		if (to) {
			to.forEach((toPath) => {
				log(`Copying ${path} with ${toPath}`);
				fs.copyFileSync(path, toPath);
			})
		} else {
			log(`Ignoring change in ${path}`);
		}
	})
	.on('unlink', (path) => log(`File ${path} has been removed`));

// More possible events.
watcher
	.on('addDir', (path) => log(`Directory ${path} has been added`))
	.on('unlinkDir', (path) => log(`Directory ${path} has been removed`))
	.on('error', (error) => log(`Watcher error: ${error}`))
	.on('ready', () => log('Initial scan complete. Ready for changes'))
	.on('raw', (event, path, details) => {
		// internal
		log('Raw event info:', event, path, details);
	});
