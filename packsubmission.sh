#!/usr/bin/env bash

if [ ! -d "punter/bin/Release/net461" ]; then
  echo "build punter first"
  exit 1
fi

cp -r punter/bin/Release/net461 _submission
pushd _submission
mv punter.exe punter
chmod a+x punter
echo "#!/usr/bin/env bash" > install
chmod a+x install
tar -cvzf ../submission.tar.gz .
popd
rm -rf _submission