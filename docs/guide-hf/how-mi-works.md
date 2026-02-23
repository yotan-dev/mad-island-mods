# How Mad Island works

In this section we will cover how Mad Island sex interactions work, at least the important bits of it,
which you need to understand in order to create your own sex interactions.

## Overview

First, it is very important to know that pretty much every sex interaction is a separate **Unity Prefab** containing the Spine object for that animation (We will call this the **Sex Prefab**).

In very simple terms, sex interactions follow the following flow:

1. Decide if we can start something (e.g.: the characters are there in expected conditions for animation)
2. Get the characters in place
3. **HIDE** the characters from the world
4. **CREATE** the Sex Prefab and "set" the character appearance on it
5. Perform the sex interaction (change between animations/etc/etc)
6. **DESTROY** the Sex Prefab
7. **SHOW** the characters in the world again

This is a very simplified overview and there are a few exceptions, examples:

1. Deliverying a baby is done with the same object as the world character
2. What "HIDE" means depends on the kind of situation, sometimes the characters are completely removed from the world, sometimes they are just visually hidden (but can still be attacked)


## Deciding to start a Sex interaction

There are 2 ways that a sex interaction may start:

1. Every few seconds, every NPC in the game checks for possible sex activity (if they want to), this usually involves checking every other character around it for whether `they can have sex` (more on that later)
	- In case of success, they will try to do it.
2. When a player triggers an action (e.g. talking or pressing `R` in an attempt to rape), this will also perform a check of whether `they can have sex`.
	- In case of success, they will try to do it.
	- In case of failure, the `Sex` button doesn't appear, or `R` does nothing.

Checking `they can have sex` is basically a way of ensuring there is an animation for that combination of characters. We call that `CanStart` condition and usually is based on:

- The characters involved (their NPC ID -- e.g. 0 for Yona, 15 for Female Native)
- The kind of sex interaction (e.g. friendly sex, rape, etc)
- The conditions of each NPC (e.g. most pregnant characters doesn't have an animation for sex with another NPC)

At this moment place and other contextual information is **not** taken into account.

If this check passes, then characters will start the sex scene.


## The Sex interaction

Now that they can start the sex scene, a place is selected and a few other info determined, so we are going into the actual sex interaction _execution_.

For that, we will have several things happening (note: sometimes, only a few does, sometimes other things happen, this is just an general presentation).

But before all of them we need to know what exactly we are going to do, because we have some contextual information that may change the general execution, for example:

- Different animations based on the Bed type
- Different animations based on quest progress

And other things you may imagine.

On HFramework, this is called a `CanExecute` condition. At this moment place and other contextual information **is** taken into account.

Now that we know what we are going to do, we can go through the process of execution.


### Get the characters in place

If needed, characters will move to the place where they will have sex, for example, going to a bed. Usually showing a heart emotion above their heads.


### **HIDE** the characters from the world

Disable the usual characters from the world (that living entity that walks, attacks, etc). Hiding may mean different things based on the needs of the interaction:

- Completely hide: Disable the NPC rendering and collider -- No one attacks them, no one sees them.
- Visually hide only: Disable the NPC rendering, but keep the collider -- Player don't see them, but they can still be attacked.

The 2nd option is interesting for example in case of a rape interaction, for example, the player is trying to rape the NPC, but another enemy is around and can attack the player to prevent that.


### **CREATE** the Sex Prefab and "set" the character appearance on it

The actual sex animation is NOT part of the character object that you see in the game world, it is a separate Spine object that is created and this moment,
and the character visuals is copied into it.

At this moment, there is also a decision of whether NPCs will get "fully focused" on the scene, or if, for example, an enemy appears nearby, they will stop what they are doing and rush to the enemy.


### Perform the sex interaction (change between animations/etc/etc)

At this moment several animations will be played, for example looping different speeds of the animation, and eventually they will update character sexual statistics (e.g. marking as having had sex, marking a creampie -- and maybe pregnancy -- etc).

This will go until the work is finished (e.g. all animations played, or player chose to "leave") or aborted due to external factors (e.g. an enemy appeared, an involved chracter died, etc)


### (Teardown) **DESTROY** the Sex Prefab

We are now done with the sex animation, so the sex prefab is destroyed.


### (Teardown) **SHOW** the characters in the world again

We are done with the sex interaction, so the characters are shown again in the world, restoring what was disabled in the HIDE step.

This marks the end of the sex interaction and some post-effects may trigger. E.g. change on Moral/friendship, etc.
