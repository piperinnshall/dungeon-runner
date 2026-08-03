#import "@preview/ambivalent-amcis:0.1.1": amcis
#import "@preview/booktabs:0.0.4": *
#import "@preview/codly:1.3.0": *
#import "@preview/codly-languages:0.1.1": *
#import "@preview/merman:0.1.0": mermaid


#let authors_list = (
  ([Riku], [Project Lead / Designer], ""), 
  ([Nathan], [Programmer], ""), 
  ([Piper], [Programmer], ""), 
  ([Kylen], [Programmer], ""),
)

#show: amcis.with(
  title: [Dungeon Runner],
  short-title: [Unity 6000.5.2f1],
  conference-line: [],
  paper-type: "Game Design Document",
  //abstract: [],
  //keywords: (),
  // acknowledgements: [],
  authors: authors_list,
  //bib: bibliography("./references.bib", style: "new-apa.csl", title: none),
  camera-ready: true,
)

#show: codly-init.with()
#codly(
  languages: codly-languages, 
  zebra-fill: none,
  stroke: none,
  display-name: false,
  lang-stroke: none,
  lang-fill: (lang) => white,
)

#show: booktabs-default-table-style

= #sym.section 1 Core

== #sym.section 1.1 Hook

A Metroidvania style game where the player explores a dungeon and looks for treasure. Escape the dungeon to buy items and explore further.

== #sym.section 1.2 Design Pillars

#table(
  columns: 3,
  align: left,
  toprule(),
  [*Pillar*], [*Meaning*], [*Consequences*],
  midrule(),
  [1. "[Pillar]"], [...], [...],
  bottomrule(),
)

== #sym.section 1.3 Core Loop

#mermaid("
flowchart LR
    A[Explore\n~60 s] --> 
    B[Fight\n5–15 s] --> 
    C[Salvage\n~10 s] --> 
    D[Upgrade\n~30 s] --> A
")

== #sym.section 1.4 Audience & Genre

Adventure dungeon crawler for people who play Zelda.

== #sym.section 1.5 Look, Feel, & Tone

Visual style for the game will be comic inspired, where player and non player
characters will be cell shaded with a near black outline that follows the geometry.
The level will have a low poly visual style somewhat similar to games on the switch/PS2.

Think: HellBoy comics meets BOTW.

== #sym.section 1.5 Goals & Non-Goals

=== Non-Goals

- No multiplayer.
- No buffs (consumables & items).
- No difficulty sliders.
- No multi-platform ports.
- No crafting.
- No swimming (death).

=== MoSCoW

#table(
  columns: 5,
  align: left,
  toprule(),
  [*Feature*], [*Priority*], [*Milestone*], [*Owner*], [*Status*],
  midrule(),
  [...], [Must], [...], [...], [Not started],
  [...], [Should], [...], [...], [Not started],
  [...], [Could], [...], [...], [Not started],
  bottomrule(),
)

#pagebreak()

== Changelog

The latest changes are tracked in the #link("https://github.com/piperinnshall/dungeon-runner/blob/main/CHANGELOG.md")[changelog].
