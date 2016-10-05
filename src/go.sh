xbuild /t:clean
xbuild
mv /var/lib/emby-server/plugins/VdrLiveTV.dll /var/lib/emby-server/plugins/VdrLiveTV.dll.prev
cp VdrLiveTV/bin/Debug/VdrLiveTV.dll /var/lib/emby-server/plugins
/etc/init.d/emby-server restart
