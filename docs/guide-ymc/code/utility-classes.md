# Utility classes

This section covers classes that provides general utilities, but are too small to deserve their own section.

Utility classes generally contain static methods that allows you to perform something or get some data.

Check their API documentation for more information.

- [CommonUtils](/api/YotanModCore.CommonUtils.html)
- [NameUtils](/api/YotanModCore.NameUtils.html)
- [StatsUtils](/api/YotanModCore.StatsUtils.html)


## Extension methods

We also include some additional methods that are "injected" into instances of existing classes, making it look like they are already part of the game.

For example, `CommonStates` has a `TakeDamage` method which can be called like that:

```C#
var dmg = new DamageInfo() { Damage = 1000 };
common.TakeDamage(dmg);
```

and this will make `common` Take 1000 damage.

See each extension API documentation for more information.

- [CommonStatesExtensions](/api/YotanModCore.Extensions.CommonStatesExtensions.html)
- [SkeletonAnimExtensions](/api/YotanModCore.Extensions.SkeletonAnimExtensions.html)
