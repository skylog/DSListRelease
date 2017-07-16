#!/bin/bash

# Запрос пароля
TF=/test
while [ ! -f $TF ]
do
  read -s -p "Введите пароль sudo: " TmpPass
  echo $TmpPass | sudo -S touch /test > /dev/null 2>&1
done
echo $TmpPass | sudo -S rm /test
SP=$TmpPass

# Скачка тугрикометра

cd ~/
echo $SP | sudo -S apt-get install sshpass
mkdir ~/.ssh
ssh-keyscan 172.16.4.130 > ~/.ssh/known_hosts
sshpass -p "Zpcc21nf" rsync -e ssh --progress -Pav --compress-level=9 user@172.16.4.130:/home/user/rsync/DS_Benchmark.tar.gz ~/.1C_wine
cd ~/.1C_wine
tar -xzvf DS_Benchmark.tar.gz
rm DS_Benchmark.tar.gz
cd DS_Benchmark/
bash benchmark.sh
