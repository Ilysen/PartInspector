# This repository has moved to https://github.com/Ilysen/PartInspectorMWC!

Despite the name, that version works on MSC as well, and represents the new modern codebase. The original description has been kept here for archival purposes.

<details><summary>Original description</summary>
# Part Inspector

This is a mod for My Summer Car that lets you look at car parts to see how damaged they are. For more info, take a look at the [Nexus page](https://www.nexusmods.com/mysummercar/mods/2291).

Part Inspector is licensed under the [GNU General Public License v3](http://www.gnu.org/licenses/agpl.html), which can be found in full in [LICENSE.md](LICENSE.md).

Feel free to use the code here for learning or reference; everything is very documented, although I still need to update the accessors to be cleaner.
I used the following resources to help make and test this mod:
- [dnSpyEx](https://github.com/dnSpyEx/dnSpy) is a very useful tool for peeking how some of the code works internally, as well as seeing how your code changes in compilation.
- [BetterCheatBox](https://www.nexusmods.com/mysummercar/mods/1679) helped a lot in testing things to make sure they worked on fresh saves.
- [Developer Toolkit](https://www.racedepartment.com/downloads/developer-toolkit.17214/) is, as far as I can tell, the only functional mod for modern versions of the game that can be used to see info about the FSMs that My Summer Car largely runs on. If you want to do any kind of modding, you'll want this or something like it. It's possible that [Developer Toolset II](https://www.nexusmods.com/mysummercar/mods/345) might work if you use the MSCLoader compatibility tool, but I haven't tried it.
- Honorable mention goes to [Modern Optimization Plugin/MOP](https://www.nexusmods.com/mysummercar/mods/146), which is by and large the only reason I can play MSC in the first place without wanting to claw my eyes out.

## Changelog

### Feb. 11, 2025
#### Version 1.3
* Now supports ground coffee, grill charcoal, fuse packages, R20 battery boxes, spark plug boxes, mosquito spray, spray cans, and fire extinguishers.
* Split up the "Display precision" setting into two options: one for parts (also includes oil filters and spark plugs), and one for items. Both settings now default to general descriptions.

### Jan. 12, 2025
#### Version 1.2.2
* Updated mod ID from `PartInspector` to `Ceres_PartInspector`.
* Touch up code quality and improve documentation.

### Dec. 24, 2025
#### Version 1.2.1
* Publicly released some unused code enable inspection of fluid containers.
* Fixed an issue making the mod incompatible with newer versions of MSCLoader.
* Built and tested on MSCLoader 1.3.

### Jun. 22, 2023
#### Version 1.1
* Now supports spark plugs and alternator belts.
* Relicensed to GPL v3. Code prior to commit `b94e1ccb8bf933c216384269b30703dc78a32342` remains licensed under the MIT License.
* Built and tested on MSCLoader 1.2.12, build 291.

### Sep. 6, 2022
#### Version 1.0
* Initial public release.
* All damageable parts can show their wear as a number, a general description, or just if they're broken or not.
* Oil filters display their dirtiness!
* Built and tested on MSCLoader 1.2.7.
</details>
