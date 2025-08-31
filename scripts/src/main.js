import { FXExtractor } from "./extractors/FXExtractor.js";
import { Project } from "./unity/Project.js";

const proj = new Project('../../Assets_v044/ExportedProject/');
await new FXExtractor().extract(proj);
