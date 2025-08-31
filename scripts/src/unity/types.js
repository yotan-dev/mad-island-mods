// This file contains general types for Unity-related data.
// MadIsland specific data is on types.mi.js

/**
 * @typedef {object} GameObject
 * @property {object} GameObject
 */

/**
 * @typedef {object} Transform
 * @property {GameObject} Transform
 */

/**
 * @typedef {(GameObject|Transform)[]} UnityYaml
 */

/**
 * @typedef {object} MonoBehaviour
 * @property {object} MonoBehaviour
 * @property {UnityMetaRef} MonoBehaviour.m_Script
 */

/**
 * @typedef {object} UnityMetaRef
 * @property {string} guid
 */

/**
 * @typedef {object} UnityMeta
 * @property {string} guid
 */



export default {};
