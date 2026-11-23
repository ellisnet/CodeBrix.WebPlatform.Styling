================================================================================
SOURCES-README: the source of the Tailwind CSS 4.3.5 programs
What the source archives beside this file hold, why they are here, and how
each one was made
================================================================================

The CodeBrix.WebPlatform.Styling.Tailwind4.<Platform> packages carry the
prebuilt standalone Tailwind CSS 4.3.5 programs exactly as Tailwind Labs
released them (tailwindcss-linux-x64, -linux-arm64, -macos-x64, -macos-arm64
and -windows-x64.exe, from
https://github.com/tailwindlabs/tailwindcss/releases/tag/v4.3.5). Each program
is a single executable that Tailwind builds with `bun build --compile` (Bun
v1.4.3): the Bun runtime, with Tailwind's JavaScript and native add-ons
embedded in it.

WHY THE SOURCE TRAVELS WITH THE PROGRAMS
----------------------------------------
The Bun runtime statically links JavaScriptCore, WTF and bmalloc from Bun's
WebKit fork, and TinyCC from Bun's TinyCC fork; Bun's own source also contains
files copied from WebKit's WebCore. Those parts are licensed under the GNU
Library/Lesser General Public License. Almost all of their LGPL files say
"version 2 of the License, or (at your option) any later version", but one
that is compiled in does not: Source/WTF/wtf/text/Base64.cpp is licensed under
the LGPL "version 2" only. So version 2's terms apply, and they ask the
distributor of the programs to accompany them with the complete source of the
library, and with the "work that uses the Library" in source or object form so
that a user can change the library and relink.

The archives below are that source. They are published on nuget.org in the
CodeBrix.WebPlatform.Styling.Tailwind4.Sources package (under sources/v4.3.5/,
with this file), and every platform package depends on that package at one
exact version - so wherever a program is downloaded, its source comes with it,
from the same place.

Not included: the tests, documentation and benchmarks of Bun and the WebKit
folders that Bun's build does not compile (they are not in anything that is
distributed), and the source of the permissively licensed libraries Bun's
build fetches by itself: the C and C++ libraries at pinned commits (its
scripts/build/deps/*.ts files, inside the Bun archive, name each repository
and commit) and the Rust crates from crates.io (pinned by its Cargo.lock).
Bun is written in Rust and C++; the MPL-2.0 crates among those are named, with
where their source is, in THIRD-PARTY-NOTICES-Tailwind.txt.


THE ARCHIVES
------------
Every archive is a plain zip. Each entry sits below one top folder named
after the source and revision; entries are in sorted order, all carry the one
timestamp given below (the source commit's date, UTC) and no file attributes
(so executable bits are not recorded - nothing in the archives is a program).
Symbolic links in a source tree are not stored; each one is listed below.

1. tailwindcss-4.3.5.zip
   sha256    bb27af0df1b87280e62bff9bd71c1cdbd4a2a466d037f85c86e7914e65a3b687
   source    https://github.com/tailwindlabs/tailwindcss, tag v4.3.5
             (commit 9b2c925c19aae46ff4f9cf54a2fa6d2dbac7fa19)
   what      The complete Tailwind CSS source of the release: the zip GitHub
             serves for the tag (the release's "Source code (zip)"), as
             downloaded - it was not remade. Its contents were checked to be
             identical to `git archive v4.3.5`.
   licences  MIT (Tailwind Labs, Inc.); crates/ignore: Unlicense OR MIT.

2. oven-sh-bun-v1.4.3.zip
   sha256    530261669eaa7061cd4c1c69ba2beccd44f271ea7ca99cbc19c7f50589d09935
   source    https://github.com/oven-sh/bun, tag bun-v1.4.3
             (commit c6da4a4d3010e5553438c60f6bd76d981976867c)
   what      The source of the Bun runtime the programs are compiled with,
             without the test/, docs/ and bench/ folders. Top folder:
             bun-bun-v1.4.3/. Timestamp 2026-10-10T01:53:32Z.
   links     not stored: AGENTS.md -> CLAUDE.md, src/AGENTS.md -> CLAUDE.md,
             src/js/AGENTS.md -> CLAUDE.md, src/cli -> runtime/cli
   licences  MIT (Oven and Bun contributors), with LGPL-2.0-or-later files
             copied from WebKit (src/jsc/bindings/webcore, src/jsc/bindings/
             webcrypto and others) and the licences of the code listed in its
             LICENSE.md.

3. oven-sh-WebKit-0c06faad-JSCOnly.zip
   sha256    495ecba149b6a436f1eb92f5cb06713c1dc5eafd941526fde7fa0de422158e47
   source    https://github.com/oven-sh/WebKit,
             commit 0c06faadf65bf8e8c8ad3a5a8aca83e1e9ed653f (WEBKIT_VERSION in
             Bun's scripts/build/deps/webkit.ts at bun-v1.4.3)
   what      The parts of WebKit that Bun's build compiles. Bun builds WebKit
             as the "JSCOnly" port with the build target `jsc`, statically;
             WebKit's Source/CMakeLists.txt then builds bmalloc, WTF,
             JavaScriptCore and (unless the system copy is used) the unifdef
             build tool. Capstone is only built when USE_CAPSTONE is set, which
             Bun's build does not do. So the archive holds:
                 CMakeLists.txt
                 Source/CMakeLists.txt
                 Source/cmake/
                 Source/bmalloc/
                 Source/WTF/
                 Source/JavaScriptCore/
                 Source/ThirdParty/unifdef/
             Top folder: WebKit-0c06faadf65bf8e8c8ad3a5a8aca83e1e9ed653f/.
             Timestamp 2026-10-06T07:17:17Z.
   licences  Mostly BSD-2-Clause (Apple Inc.) with LGPL-2.0-or-later files;
             Source/WTF/wtf/text/Base64.cpp LGPL-2.0-only; Source/WTF/wtf/
             DateMath.h and Source/JavaScriptCore/runtime/JSDateMath.h
             MPL-1.1 / GPL-2.0-or-later / LGPL-2.1-or-later; other files under
             the licences their headers state.

4. oven-sh-tinycc-05f0fafa.zip
   sha256    f141c6ad0b823bed68d21fa6b104e4a5df9de8eaecd1e2c3b5abbee24eff40be
   source    https://github.com/oven-sh/tinycc,
             commit 05f0fafaa3be31e31d7b4b5c17dc60f62c991171 (TINYCC_COMMIT in
             Bun's scripts/build/deps/tinycc.ts at bun-v1.4.3)
   what      The complete repository at that commit. Top folder:
             tinycc-05f0fafaa3be31e31d7b4b5c17dc60f62c991171/.
             Timestamp 2026-07-20T05:20:38Z.
   licences  LGPL-2.1-or-later (COPYING), with the other notices in the tree.

SOURCES-LICENSES.txt, beside this file, is the package's licence file: the
licences of each archive in short.


RELINKING
---------
Bun documents how to build Bun against a changed WebKit (its licensing page,
https://bun.com/docs/project/licensing): clone Bun's WebKit fork into
vendor/WebKit, run `bun sync-webkit-source` to check out the pinned
WEBKIT_VERSION, then `bun run build:local`. The programs themselves are then
rebuilt from the Tailwind source with packages/@tailwindcss-standalone
(`bun ./scripts/build.ts`), using the new Bun.


HOW THE ARCHIVES WERE MADE (and how to remake them byte for byte)
-----------------------------------------------------------------
Archives 2 to 4 were made with tools/source-zips/MakeSourceZip.cs (a .NET 10
file-based program in this repository). The same input gives the same bytes:
sorted entries, one fixed timestamp, no attributes, and the zlib that .NET
carries at CompressionLevel.SmallestSize. From the repository root, after
getting each source:

  Bun:     download https://github.com/oven-sh/bun/archive/refs/tags/bun-v1.4.3.tar.gz
           and extract it (folder bun-bun-v1.4.3), then
           dotnet run tools/source-zips/MakeSourceZip.cs -- --root <folder>
             --prefix bun-bun-v1.4.3/ --time 2026-10-10T01:53:32Z
             --out info/v4.3.5/oven-sh-bun-v1.4.3.zip --include .
             --exclude test --exclude docs --exclude bench

  WebKit:  git init <folder>; cd <folder>
           git remote add origin https://github.com/oven-sh/WebKit.git
           git fetch --depth 1 --filter=blob:none origin 0c06faadf65bf8e8c8ad3a5a8aca83e1e9ed653f
           git sparse-checkout set --no-cone /CMakeLists.txt /Source/CMakeLists.txt
             /Source/cmake/ /Source/bmalloc/ /Source/WTF/ /Source/JavaScriptCore/
             /Source/ThirdParty/unifdef/
           git checkout FETCH_HEAD
           then, from the repository root:
           dotnet run tools/source-zips/MakeSourceZip.cs -- --root <folder>
             --prefix WebKit-0c06faadf65bf8e8c8ad3a5a8aca83e1e9ed653f/
             --time 2026-10-06T07:17:17Z
             --out info/v4.3.5/oven-sh-WebKit-0c06faad-JSCOnly.zip
             --include CMakeLists.txt --include Source/CMakeLists.txt
             --include Source/cmake --include Source/bmalloc --include Source/WTF
             --include Source/JavaScriptCore --include Source/ThirdParty/unifdef

  TinyCC:  download https://github.com/oven-sh/tinycc/archive/05f0fafaa3be31e31d7b4b5c17dc60f62c991171.tar.gz
           and extract it, then
           dotnet run tools/source-zips/MakeSourceZip.cs -- --root <folder>
             --prefix tinycc-05f0fafaa3be31e31d7b4b5c17dc60f62c991171/
             --time 2026-07-20T05:20:38Z
             --out info/v4.3.5/oven-sh-tinycc-05f0fafa.zip --include .

The build of the .Sources package checks all four archives against the sha256
values above before it packs anything.


THE OTHER FILES HERE
--------------------
  urls.txt         The release files of Tailwind CSS 4.3.5 with their sha256
                   values - for reference; the build has the five program
                   URLs and hashes written into the platform projects.
  sha256sums.txt   The release's own checksum file, as published.

================================================================================
