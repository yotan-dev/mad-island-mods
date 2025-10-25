import { FXExtractor } from "./extractors/FXExtractor.js";
import { Project } from "./unity/Project.js";
import { SoundExtractor } from "./extractors/SoundExtractor.js";
import { NpcIdExtractor } from "./extractors/NpcIdExtractor.js";
import { TagExtractor } from "./extractors/TagExtractor.js";
import { LayerExtractor } from "./extractors/LayerExtractor.js";
import { TextColorExtractor } from "./extractors/TextColorExtractor.js";

const path = process.argv[2];
if (!path) {
	console.error('Please provide a path to the Assets folder. Usage: npm run generate ../Path/to/Assets/folder/');
	process.exit(1);
}

const proj = new Project(path);
await new NpcIdExtractor().extract(proj);
await new FXExtractor().extract(proj);
await new SoundExtractor().extract(proj);
await new TagExtractor().extract(proj);
await new LayerExtractor().extract(proj);
await new TextColorExtractor().extract(proj);
