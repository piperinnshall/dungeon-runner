#import "@preview/ambivalent-amcis:0.1.1": amcis
#import "@preview/booktabs:0.0.4": *
#import "@preview/codly:1.3.0": *
#import "@preview/codly-languages:0.1.1": *
#import "@preview/merman:0.1.0": mermaid


#let authors_list = (
  ([Riku], [Design Lead], ""), 
  ([Nathan], [Design Lead], ""), 
  ([Piper], [Design Lead], ""), 
  ([Kylen], [Design Lead], ""),
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

For [audience] who want [experience], [title] is a [genre] where [the one thing no other game does].

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
    A[Explore\n~60 s] --> B[Fight\n5–15 s] --> C[Salvage\n~10 s] --> D[Upgrade\n~30 s] --> A
")

== #sym.section 1.4 Audience & Genre

== #sym.section 1.5 Look, Feel, & Tone

== #sym.section 1.5 Goals & Non-Goals

=== Non-Goals

- No ...

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

= Changelog

#include "Changelog.typ"

