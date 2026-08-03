#import "@preview/booktabs:0.0.4": *
#show: booktabs-default-table-style

#table(
  columns: 4,
  align: left,
  toprule(),
  [*Version*], [*Date*], [*Change*], [*Who*],
  midrule(),
  [v0.1], [YYYY-MM-DD], [Initial Change], [GitHub Username],
  bottomrule(),
)
