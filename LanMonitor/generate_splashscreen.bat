@Echo OFF

CHCP 65001>NUL

convert splashscreen_original.png -font Verdana -fill #084C94 -gravity Northwest -interline-spacing 4 -pointsize 12 -annotate +105+270 "Version: V0.2.3\nAuthor: Apollo Wayne\n© 2022 CRSCD" splashscreen.png