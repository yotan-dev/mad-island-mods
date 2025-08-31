import { FXExtractor } from "./extractors/FXExtractor.js";
import { Project } from "./unity/Project.js";
import { SoundExtractor } from "./extractors/SoundExtractor.js";
import { NpcIdExtractor } from "./extractors/NpcIdExtractor.js";

const proj = new Project('../../Assets_v044/');
await new NpcIdExtractor().extract(proj);
await new FXExtractor().extract(proj);
await new SoundExtractor().extract(proj);

