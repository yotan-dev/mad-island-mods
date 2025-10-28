# Damage Type

This list the known DamageType values used around the game. While this should give a general idea, it is far from complete.

There are a lot of damageType interactions in `DamageTrigger.Effect`, and it triggers other code for specific things.


## damageType 0

Default damage, it used an attack power rate when coming from weapons/etc (A NPC is performing the attack) or flat damage when comming from objects.

Example items:
- arrow_00: deals damage by 30% of attack power
- arrow_chaos_01: deals damage by 100% of attack power ; 25% chance to shoot 2 arrows (extra code?)


## damageType 1

Stun/Faint damage.

Example items:
- arrow_pretty_01: deals stun damage by bows attack power
- purple needles


## damageType 2
- unused

## damageType 3
- deals damage by 80% of attack power; Slow down the enemy's next move (Arrow Sprider 01)

## damageType 4
- fire from wall in ruins

## damageType 5
- pendulum from ruins

## damageType 6
- unused

## damageType 7
- ground spikes from ruins

## damageType 8
- iron ball from ruins

## damageType 9
- arrows from wall in ruins

## damageType 10
- fall from ruins

## damageType 11
- explosive plant from jungle

## damageType 12
- drug bomb smoke (purple)

## damageType 13
- poison mist (chaos area?)

## damageType 14
- cobweb paralysis (I think this is more focused on the paralysis itself)

## damageType 15
- ground healing
