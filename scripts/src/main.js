import { FXExtractor } from "./extractors/FXExtractor.js";
import { Project } from "./unity/Project.js";
import { SoundExtractor } from "./extractors/SoundExtractor.js";
import { NpcIdExtractor } from "./extractors/NpcIdExtractor.js";
import { TagExtractor } from "./extractors/TagExtractor.js";
import { LayerExtractor } from "./extractors/LayerExtractor.js";
import { TextColorExtractor } from "./extractors/TextColorExtractor.js";

const proj = new Project('../../Assets_v044/');
await new NpcIdExtractor().extract(proj);
await new FXExtractor().extract(proj);
await new SoundExtractor().extract(proj);
await new TagExtractor().extract(proj);
await new LayerExtractor().extract(proj);
await new TextColorExtractor().extract(proj);
