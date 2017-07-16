
cp /etc/scripts/CALL/restart_sip_client.desktop "/home/user/Рабочий стол/"
chown user. "/home/user/Рабочий стол/restart_sip_client.desktop"
echo "#!/bin/sh" > /etc/scripts/CALL/hangup.sh
echo "linphonecsh hangup" >> /etc/scripts/CALL/hangup.sh
chmod +x /etc/scripts/CALL/hangup.sh

cat << 'EOF' > /etc/scripts/CALL/hangup.desktop
#!/usr/bin/env xdg-open
[Desktop Entry]
Version=1.0
Type=Application
Terminal=true
Icon[ru_RU]=system-run
Name[ru_RU]=Сброс звонка
Name=Сброс звонка
Exec="/etc/scripts/CALL/hangup.sh"
Icon=system-run
EOF

chmod +x /etc/scripts/CALL/hangup.desktop
cp /etc/scripts/CALL/hangup.desktop "/home/user/Рабочий стол/"
chown user. "/home/user/Рабочий стол/hangup.desktop"

mkdir -p /home/user/.config/autostart
chown user. /home/user/.config/autostart
cp /etc/scripts/CALL/linphone.desktop /home/user/.config/autostart
chown user. /home/user/.config/autostart/linphone.desktop
