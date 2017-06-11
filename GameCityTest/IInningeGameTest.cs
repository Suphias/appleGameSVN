﻿using System;
using Xunit;
using AntDesigner.NetCore.GameCity;
using Moq;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameCityTest
{
    public class IInningeGameTest
    {
        IGameProject _gameProject;
        Mock<IGameProject> _gameProjectFactory;
        public IInningeGameTest()
        {
           Mock_gameProject();

        }
        private void Mock_gameProject()
        {
            _gameProjectFactory = new Mock<IGameProject>();
          
           _gameProject = _gameProjectFactory.Object;
            _gameProjectFactory.Setup(g => g.CheckAddSeat(It.IsNotNull<IInningeGame>())).Returns(true);
            _gameProjectFactory.SetupAllProperties();
           
            _gameProject.PlayerCountLeast = 1;
            _gameProject.PlayerCountLimit = 10;
        }
        private IInningeGame _IInningeGameCreator()
        {
            IInningeGame inningeGame = new InningeGame()
            {
                IGameProject = _gameProject
            };
            return inningeGame;
        }
        [Theory(DisplayName ="启动游戏_座位未达最低要求")]
        [InlineData(4,3)]
        public void StartGame_SeatNotEnough_(int playerCount_,int seatCount_)
        {

            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLeast= playerCount_;
            inningeGame.IGameProject.PlayerCountLimit = playerCount_;

            inningeGame.AddSet(seatCount_);
            var gameHaveStarted=inningeGame.Start();

            Assert.False (inningeGame.IsStarted);
            Assert.False(gameHaveStarted);
        }
        [Theory(DisplayName = "启动游戏_座位达最低要求_座位未超最高限制")]
        [InlineData(3,3,3)]
        [InlineData(3,4,4)]
        [InlineData(1,4,2)]
        [InlineData(0,2,1)]
        public void StartGame_SeatEnough_(int playerCount_,int limitCount_, int seatCount_)
        {
            
            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLeast = playerCount_;
            inningeGame.IGameProject.PlayerCountLimit = limitCount_;

            inningeGame.AddSet(seatCount_);
            var gameHaveStarted = inningeGame.Start();

            Assert.True (inningeGame.IsStarted);
            Assert.True(gameHaveStarted);
        }

        [Theory(DisplayName = "启动游戏_座位为零")]
        [InlineData(1,0)]
        [InlineData(0,0)]
        public void StartGame_SeatIsZero_(int playerCount_, int seatCount_)
        {
            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLeast = playerCount_;

            inningeGame.AddSet(seatCount_);

            Assert.Throws<Exception>(() => inningeGame.Start());
        }
        [Theory(DisplayName ="添加一个空座位_座位已达到游戏上限_异常")]
        [InlineData(5,5)]
        [InlineData(1,1)]
        [InlineData(0,0)]
        public void AddSeat_NoEmptySeat(int playerCountLimit_,int seatCount_)
        {
           IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLimit = playerCountLimit_;

            inningeGame.AddSet(seatCount_);
            int preSeatCount = inningeGame.SeatCount;

            Assert.Throws<Exception>(() => inningeGame.AddSet(1));
        }
        [Fact(DisplayName ="返回空座位_")]
        public void GetEmptySeats()
        {
            IInningeGame inningeGame= new InningeGame();
            List<ISeat> Seats= inningeGame.EmptySeats();
            Assert.Empty(Seats); 
        }
        [Theory(DisplayName = "1返回空座位_有n个空座位")]
        [InlineData(2)]
        [InlineData(1)]
        [InlineData(0)]
        public void GetEmptySeats_n(int emptyCount_)
        {
            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLimit = emptyCount_;
            inningeGame.IGameProject.PlayerCountLeast = emptyCount_;

            inningeGame.AddSet(emptyCount_);
            List<ISeat> Seats = inningeGame.EmptySeats();

            Assert.True(Seats.Count == emptyCount_);
        }
        [Fact(DisplayName ="2返回一个空座位")]
        public void PlayerSitDown()
        {
            IInningeGame inningeGame = _IInningeGameCreator();

            inningeGame.AddSet(1);
            ISeat emptySeat= inningeGame.GetOneEmptySeat();
            Assert.NotNull(emptySeat);
        }
        [Fact(DisplayName = "委托检查能否启动游戏_能")]
        public void Start_DCheckStart_yes()
        {
            var canStart = false;
            IInningeGame inningeGame = _IInningeGameCreator();

            inningeGame.DCheckStart += delegate { canStart = true; return true; };
            inningeGame.AddSet(1);
            var gameHaveStarted = inningeGame.Start();

            Assert.True(gameHaveStarted);
            Assert.True(canStart);
        }
        [Fact(DisplayName = "委托检查能否启动游戏_不能")]
        public void Start_DCheckStart_no()
        {
            IInningeGame inningeGame = _IInningeGameCreator();

            var canStart = true;
            inningeGame.DCheckStart += delegate { canStart = false; return false ;};
            inningeGame.AddSet(1);
            var gameHaveStarted = inningeGame.Start();

            Assert.False(gameHaveStarted);
            Assert.False(canStart);
        }
        [Fact(DisplayName = "触发启动游戏事件")]
        public void Start_Event()
        {
            var beforGameStartEvent = false;
            var StartEvent = false;
            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.BeforGameStartHandler += delegate { beforGameStartEvent = true; };
            inningeGame.GameStartHandler += delegate { StartEvent = true; };

            inningeGame.AddSet(1);
            var gameHaveStarted = inningeGame.Start();

            Assert.True(beforGameStartEvent, "启动游戏前");
            Assert.True(StartEvent,"启动游戏事件");
        }
        [Theory(DisplayName = "委托检查能否添加座位")]
        [InlineData(5, 5)]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        public void AddSeat_DCheckAddRoom(int playerCountLimit_, int seatCount_)
        {
            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLimit = playerCountLimit_;

            bool DCheckAddRoom = false;
            inningeGame.DCheckAddSeat+= delegate { DCheckAddRoom = true; return true; };
            inningeGame.AddSet(seatCount_);
            int preSeatCount = inningeGame.SeatCount;

            Assert.True(DCheckAddRoom);
        }
        [Theory(DisplayName = "触发添加座位前后事件")]
        [InlineData(5, 5)]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        public void AddSeat_AddSeatEvent(int playerCountLimit_, int seatCount_)
        {    
            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLimit = playerCountLimit_;

            bool beforAddSeatEvent = false;
            bool afterAddSeatEvent = false;
            inningeGame.BeforAddSeatHandler += delegate { beforAddSeatEvent = true; };
            inningeGame.AfterAddSeatHandler += delegate { afterAddSeatEvent = true; };
            inningeGame.AddSet(seatCount_);
            int preSeatCount = inningeGame.SeatCount;

            Assert.True(beforAddSeatEvent,"添加座位前");
            Assert.True(afterAddSeatEvent, "添加座位后");
        }

        [Fact(DisplayName = "添加座位前后事件被IGameProject捕获执行")]
        public void ConnectIInngeGameAndIGameProject_AddSet()
        {
            IInningeGame inningeGame = new InningeGame(_gameProject);
            inningeGame.AddSet(1);

            _gameProjectFactory.Verify(g => g.BeforAddSeat(It.IsNotNull<IInningeGame>(), It.IsNotNull<EventArgs>()), "添加座位前");
            _gameProjectFactory.Verify(p => p.AfterAddSeat(It.IsNotNull<object>(), It.IsNotNull<EventArgs>()), "添加座位后");
        }
        [Fact(DisplayName = "IGameProject执行游戏能否启动委托检查被")]
        public void ConnectIInngeGameAndIGameProject_startCheck()
        {
            IInningeGame inningeGame = new InningeGame(_gameProject);
  
            inningeGame.AddSet(1);
            inningeGame.Start();

            _gameProjectFactory.Verify(g => g.CheckStart(It.IsNotNull<IInningeGame>()));
        }
        [Fact(DisplayName ="IGameProject执行游戏启动委托")]
        public void ConnectIInngeGameAndIGameProject_startDelegate()
        {
            IInningeGame inningeGame = new InningeGame(_gameProject);
           _gameProjectFactory.Setup(g => g.CheckStart(It.IsNotNull<IInningeGame>())).Returns(true);

            inningeGame.AddSet(1);
            inningeGame.Start();

            _gameProjectFactory.Verify(g => g.GameStart(It.IsNotNull<IInningeGame>(), It.IsNotNull<EventArgs>()));
        }
        [Theory(DisplayName = "返回不空的座位_有n个座位")]
        [InlineData(2)]
        [InlineData(1)]
        [InlineData(0)]
        public void GetNotEmptySeats_n(int notEmptyCount_)
        {

            Mock<IPlayerJoinRoom> playerFactory = new Mock<IPlayerJoinRoom>();

            IInningeGame inningeGame = _IInningeGameCreator();
            inningeGame.IGameProject.PlayerCountLimit = notEmptyCount_;
            inningeGame.IGameProject.PlayerCountLeast = notEmptyCount_;

            inningeGame.AddSet(notEmptyCount_);

            List<ISeat> Seats = inningeGame.EmptySeats();
            while (inningeGame.GetOneEmptySeat()!=null)
            {
                inningeGame.GetOneEmptySeat().PlayerSitDown(playerFactory.Object);
            }
           
            Assert.True(Seats.Count == notEmptyCount_);
        }
    }
}
