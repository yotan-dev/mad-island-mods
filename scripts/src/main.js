import { FXExtractor } from "./extractors/FXExtractor.js";
import { Project } from "./unity/Project.js";
import { SoundExtractor } from "./extractors/SoundExtractor.js";

const proj = new Project('../../Assets_v044/ExportedProject/');
await new FXExtractor().extract(proj);
await new SoundExtractor().extract(proj);

