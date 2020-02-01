@echo off

"SourceUtils.WebExport\bin\Debug\SourceUtils.WebExport.exe" export ^
	--maps "surf_mesa" ^
	--outdir "..\MOMReplayViewer-pages\resources" ^
	--gamedir "C:\Program Files (x86)\Steam\steamapps\common\Momentum Mod\momentum" ^
	--mapsdir "maps" ^
	--overwrite --verbose --url-prefix "/MOMReplayViewer/resources" ^
	--packages "../hl2/hl2_textures_dir.vpk, ../hl2mp/hl2mp_pak_dir.vpk"
