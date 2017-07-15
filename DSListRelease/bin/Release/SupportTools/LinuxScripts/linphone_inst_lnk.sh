#!/bin/bash
export DISPLAY=:0
cp /etc/scripts/CALL/hangup.desktop /usr/share/applications/
chmod 644 /usr/share/applications/hangup.desktop 
CURRENTL=`su - user -c "gsettings get org.cinnamon panel-launchers | sed 's/\[//g' | sed 's/\]//g'"`
NEWL="[$CURRENTL, 'hangup.desktop']"
su - user -c "gsettings set org.cinnamon panel-launchers \"$NEWL\""
