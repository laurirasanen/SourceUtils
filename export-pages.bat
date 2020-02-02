@echo off

"SourceUtils.WebExport\bin\x64\Debug\SourceUtils.WebExport.exe" export ^
	--maps "bhop_*,rj_*,surf_*" ^
	--outdir "..\MomReplayViewer-pages\resources" ^
	--gamedir "C:\Program Files (x86)\Steam\steamapps\common\Momentum Mod\momentum" ^
	--mapsdir "maps" ^
	--overwrite --verbose --url-prefix "./resources" ^
	--packages "sourceutilvpks/hl2_pak_dir.vpk,sourceutilvpks/hl2mp_pak_dir.vpk,sourceutilvpks/cstrike_pak_dir.vpk,sourceutilvpks/tf2_textures_dir.vpk"

PAUSE