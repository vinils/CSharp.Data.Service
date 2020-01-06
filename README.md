# CSharp.Data.Service
Data Service

[![CircleCI](https://circleci.com/gh/vinils/CSharp.Data.Service.svg?style=svg)](https://circleci.com/gh/vinils/CSharp.Data.Service)

*use git clone --recurse-submodules https://github.com/vinils/CSharp.Saude.FitbitTask.git

docker build -t data_app .
docker run -e "DataContext=Persist Security Info=False;User ID=sa;Password=Senh@123;Data Source=w19docker6,1433;MultipleActiveResultSets=True; Initial Catalog=DataContext" -p 8002:80 -p 8003:443 -d data_app
