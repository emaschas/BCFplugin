:: WIX

candle BCFplugin.wxs
@if ERRORLEVEL 1 pause

light  BCFplugin.wixobj
@if ERRORLEVEL 1 pause

@del /Q BCFplugin.wixpdb BCFplugin.wixobj