/// <reference path="../api/jquery-1.7.1.min.js" />
/// <reference path="../api/jquery.validate.min.js" />
/// <reference path="../api/knockout-2.0.0.js" />
/// <reference path="../api/jquery-ui-1.8.16.min.js" />
/// <reference path="../api/jquery.autoresize.js" />
/// <reference path="../api/date.js" />
/// <reference path="../api/jquery.hotkeys.js" />
/// <reference path="../api/tag-it.js" />
/// <reference path="../core/ResourceAllocation.js" />

ResourceAllocationTest = TestCase("ResourceAllocationTest");

ResourceAllocationTest.prototype.testDependentCollectionsVM = function () {
    console.log("starting testDependentColl");
    var obj = {
        "Teams":
            [{
                "Id": 1,
                "Name": "Management",
                "Members": [
            {
                "Id": 2, "FullName": "David Neigler",
                "Projects": [
                    {
                        "Id": 39, "Name": "Management",
                        "MonthlyAllocations": [
                            { "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 }
                        ]
                    },
                    {
                        "Id": 40, "Name": "Project 2",
                        "MonthlyAllocations": [
                            { "Month": "\/Date(1293858000000)\/", "Id": 999, "Allocation": 1.00000 }
                        ]
                    }
                ]
            }],
                "LeadFullName": "David Neigler",
                "LeadId": "2"
            }],
        "Months": ["\/Date(1293858000000)\/", "\/Date(1296536400000)\/", "\/Date(1298955600000)\/", "\/Date(1301630400000)\/", "\/Date(1304222400000)\/", "\/Date(1306900800000)\/", "\/Date(1309492800000)\/", "\/Date(1312171200000)\/", "\/Date(1314849600000)\/", "\/Date(1317441600000)\/", "\/Date(1320120000000)\/", "\/Date(1322715600000)\/", "\/Date(1325394000000)\/", "\/Date(1328072400000)\/"]
    };
    var at = new allocationTree(obj);
    assertTrue(at._dependentCollections().length > 0);

    assertFalse(at.ChangeLog().length > 0);
    
    var origTeam = at.Teams()[0].Name();
    var tm1 = at.Teams()[0];

    tm1.Name("Updated team name");
    assertTrue(tm1.HasChanges());
    assertTrue($.inArray("Name", tm1.ChangeLog()) >= 0);
    assertEquals(origTeam, tm1.OriginalVersion["Name"]);
    assertEquals(1, tm1.Members()[0].Projects()[0].Allocations()[0].Allocation());
    assertEquals("Updated team name", tm1.Name());

    // revert to original value
    tm1.Name(origTeam);

    assertEquals(origTeam, tm1.OriginalVersion["Name"]);
    assertFalse($.inArray("Name", tm1.ChangeLog()) >= 0);
    assertEquals(0, tm1.ChangeLog().length);

    assertTrue(tm1._dependentCollectionsLoaded);
    if (tm1.HasChildChanges()) {
        ko.utils.arrayForEach(tm1, function (tm) {
            console.log(tm);
        });
    }
    assertFalse(tm1.HasChildChanges2());

    assertFalse(tm1.HasChanges2());

    assertNotNull(tm1.Members()[0].Projects()[0].Allocations()[0].Parent());

    var origValue = tm1.Members()[0].Projects()[0].Allocations()[0].Allocation();

    // update allocation
    tm1.Members()[0].Projects()[0].Allocations()[0].Allocation(.5);

    // we aren't tracking team changes, so the Updaeted name shouldn't register
    // assertFalse(at.Teams()[0].HasChanges());
    // we are tracking allocation changes, so this should register

    assertTrue(at.Teams()[0].Members()[0].Projects()[0].Allocations()[0].HasChanges2());
    assertTrue(at.Teams()[0].Members()[0].Projects()[0].HasChanges2());
    assertTrue(at.Teams()[0].Members()[0].HasChanges2());

    // revert change
    at.Teams()[0].Members()[0].Projects()[0].Allocations()[0].Allocation(origValue);
    assertEquals(1, tm1.Members()[0].Projects()[0].Allocations()[0].Allocation());

    assertFalse(at.Teams()[0].Members()[0].Projects()[0].Allocations()[0].HasChanges2());

    assertFalse(at.Teams()[0].Members()[0].HasLocalChanges());

    assertFalse(at.Teams()[0].Members()[0].HasChanges2());
    
    at.Teams()[0].Members()[0].Projects()[0].Allocations()[0].Allocation(.5);
    
    assertTrue(at.Teams()[0].Members()[0].Projects()[0].Allocations()[0].HasChanges2());

    assertTrue(at.Teams()[0].Members()[0].Projects()[0].HasChanges2());

    // haschanges should make its way all up the stack
    assertTrue(at.Teams()[0].Members()[0].HasChanges2());
    // the child allocation change should bubble all the way up.
    assertTrue(at.HasChanges2());
};

ResourceAllocationTest.prototype.testResourceAllocationVM = function () {
    var obj = {"Teams":
            [{ "Id": 1,
            "Name": "Management",
            "Members": [
            { "Id": 2, "FullName": "David Neigler",
                "Projects": [
                    { "Id": 39, "Name": "Management",
                        "MonthlyAllocations": [
                            { "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 }
                        ]
                    },
                    { "Id": 40, "Name": "Project 2",
                        "MonthlyAllocations": [
                            { "Month": "\/Date(1293858000000)\/", "Id": 999, "Allocation": 1.00000 }
                        ]
                    }
                ]
            }],
            "LeadFullName": "David Neigler",
            "LeadId": "2"
        }],
        "Months":["\/Date(1293858000000)\/","\/Date(1296536400000)\/","\/Date(1298955600000)\/","\/Date(1301630400000)\/","\/Date(1304222400000)\/","\/Date(1306900800000)\/","\/Date(1309492800000)\/","\/Date(1312171200000)\/","\/Date(1314849600000)\/","\/Date(1317441600000)\/","\/Date(1320120000000)\/","\/Date(1322715600000)\/","\/Date(1325394000000)\/","\/Date(1328072400000)\/"]};
    var at = new allocationTree(obj);
    assertEquals("Teams.Count", 1, at.Teams().length);
    
};
ResourceAllocationTest.prototype.testTeamLoadFromObject = function () {
    var teamObj = {
        "Id": 1,
        "Name": "Management",
        "Members": [
            {
                "Id": 2, "FullName": "David Neigler",
                "Projects": [
                {
                    "Id": 39, "Name": "Management",
                    "MonthlyAllocations": [
                    { "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 },
                    { "Month": "\/Date(1296536400000)\/", "Id": 998, "Allocation": 1.00000 },
                    { "Month": "\/Date(1298955600000)\/", "Id": 999, "Allocation": 1.00000 },
                    { "Month": "\/Date(1301630400000)\/", "Id": 1000, "Allocation": 1.00000 },
                    { "Month": "\/Date(1304222400000)\/", "Id": 1001, "Allocation": 1.00000 },
                    { "Month": "\/Date(1306900800000)\/", "Id": 1002, "Allocation": 1.00000 },
                    { "Month": "\/Date(1309492800000)\/", "Id": 1003, "Allocation": 1.00000 },
                    { "Month": "\/Date(1312171200000)\/", "Id": 1004, "Allocation": 1.00000 },
                    { "Month": "\/Date(1314849600000)\/", "Id": 1005, "Allocation": 1.00000 },
                    { "Month": "\/Date(1317441600000)\/", "Id": 1006, "Allocation": 1.00000 },
                    { "Month": "\/Date(1320120000000)\/", "Id": 1007, "Allocation": 1.00000 },
                    { "Month": "\/Date(1322715600000)\/", "Id": 1008, "Allocation": 1.00000 },
                    { "Month": "\/Date(1325394000000)\/", "Id": 1009, "Allocation": 1.00000 },
                    { "Month": "\/Date(1328072400000)\/", "Id": 1010, "Allocation": 1.00000 },
                    { "Month": "\/Date(1330578000000)\/", "Id": 1011, "Allocation": 1.00000 }]
                }]
            }],
        "LeadFullName": "David Neigler",
        "LeadId": "2"
    };
    var team1 = new team(teamObj);

    //assertEquals("Team.Members count before", 0, team1.Members().length);
    //team1.LoadFromObject(
    //    );

    assertEquals("Team.Id", 1, team1.Id());
    assertEquals("Team.Name", "Management", team1.Name());
    assertEquals("Team.LeadFullName", "David Neigler", team1.LeadFullName());
    assertEquals("Team.LeadId", 2, team1.LeadId());
    assertEquals("Team.Members Count", 1, team1.Members().length);
    assertEquals("Team.Members[0]", 2, team1.Members()[0].Id());
    assertEquals("Team.Members[0].Projects[0]", 39, team1.Members()[0].Projects()[0].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[0]", 997, team1.Members()[0].Projects()[0].Allocations()[0].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[1]", 998, team1.Members()[0].Projects()[0].Allocations()[1].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[2]", 999, team1.Members()[0].Projects()[0].Allocations()[2].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[3]", 1000, team1.Members()[0].Projects()[0].Allocations()[3].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[4]", 1001, team1.Members()[0].Projects()[0].Allocations()[4].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[5]", 1002, team1.Members()[0].Projects()[0].Allocations()[5].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[6]", 1003, team1.Members()[0].Projects()[0].Allocations()[6].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[7]", 1004, team1.Members()[0].Projects()[0].Allocations()[7].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[8]", 1005, team1.Members()[0].Projects()[0].Allocations()[8].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[9]", 1006, team1.Members()[0].Projects()[0].Allocations()[9].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[10]", 1007, team1.Members()[0].Projects()[0].Allocations()[10].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[11]", 1008, team1.Members()[0].Projects()[0].Allocations()[11].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[12]", 1009, team1.Members()[0].Projects()[0].Allocations()[12].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[13]", 1010, team1.Members()[0].Projects()[0].Allocations()[13].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[14]", 1011, team1.Members()[0].Projects()[0].Allocations()[14].Id());
};

ResourceAllocationTest.prototype.testProjectLoadFromObject = function () {
    var projObj = {
        "Id": 39, "Name": "Management",
        "MonthlyAllocations": [
                    { "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 },
                    { "Month": "\/Date(1296536400000)\/", "Id": 998, "Allocation": 1.00000 }
        ]
    };
    var proj = new project(projObj);

    //proj.LoadFromObject(
    //    );

    //$.each(proj.Allocations(), function (x, item) {
    //    console.log("ProjectLoadFromObject", item.Id());
    //});

    assertEquals("MA", 997, proj.Allocations()[0].Id());
    assertEquals("Project.Id", 39, proj.Id());
    assertEquals("Project.Name", "Management", proj.Name());
    assertTrue("Project.Alloc 997 Exists", proj.AllocationExists(997));
    assertTrue("Project.Alloc 998 Exists", proj.AllocationExists(998));
    assertEquals("Project.MonthlyAllocations", 2, proj.Allocations().length);
     assertFalse("Project.Alloc 999 NOT Exists", proj.AllocationExists(999));
    

};

ResourceAllocationTest.prototype.testEntityObjectExists = function () {
    var obj = new entityObject();
    var arr = ko.observableArray([]);
    arr.push(new monthlyAllocation({ "Month": Date.today(), "Id": 1, "Allocation": 1.00000 }));
    arr.push(new monthlyAllocation({ "Month": Date.today(), "Id": 2, "Allocation": 1.00000 }));
    arr.push(new monthlyAllocation({ "Month": Date.today(), "Id": 3, "Allocation": 1.00000 }));
    assertTrue("1 should exist, accessed with wrapped array", obj.Exists(arr, 1));
    assertTrue("2 should exist, accessed with unwrapped array", obj.Exists(arr(), 2));
    assertTrue("3 should exist", obj.Exists(arr, 3));
    assertFalse("4 should not exist", obj.Exists(arr, 4));
};

ResourceAllocationTest.prototype.testMonthlyAllocationLoadFromObject = function () {
    var alloc = new monthlyAllocation({ "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 });

    assertEquals("Alloc.Id", 997, alloc.Id());
    assertEquals("Alloc.Allocation", 1, alloc.Allocation());
    assertEquals("Alloc.Month", /Date(1293858000000)/, alloc.Month());

    var alloc2 = new monthlyAllocation({ "Month": "\/Date(1296536400000)\/", "Id": 998, "Allocation": 1.00000 });
    
    assertEquals("Alloc.Id", 998, alloc2.Id());
    assertEquals("Alloc.Allocation", 1, alloc2.Allocation());
    assertEquals("Alloc.Month", /Date(1296536400000)/, alloc2.Month());
};

ResourceAllocationTest.prototype.testVMInit = function () {
    assertNotUndefined("alloc vm should not be null", resourceAllocationVM);
    assertNotUndefined("alloc tree should not be null", resourceAllocationVM.AllocationTree);
    assertEquals("teams length == 1", 1, resourceAllocationVM.AllocationTree().Teams().length);

};


ResourceAllocationTest.prototype.testTeamLoadWithZeroAllocations = function () {
    var teamObj = {
        "Id": 1,
        "Name": "Management",
        "Members": [
            {
                "Id": 2, "FullName": "David Neigler",
                "Projects": [
                {
                    "Id": 39, "Name": "Management",
                    "MonthlyAllocations": [
                    { "Month": "\/Date(1293858000000)\/", "Id": 997, "Allocation": 1.00000 },
                    { "Month": "\/Date(1296536400000)\/", "Id": 998, "Allocation": 1.00000 },
                    { "Month": "\/Date(1298955600000)\/", "Id": 0, "Allocation": 0.00000 },
                    { "Month": "\/Date(1301630400000)\/", "Id": 0, "Allocation": 0.00000 },
                    { "Month": "\/Date(1304222400000)\/", "Id": 0, "Allocation": 0.00000 }]
                }]
            }],
        "LeadFullName": "David Neigler",
        "LeadId": "2"
    };
    var team1 = new team(teamObj);

    //assertEquals("Team.Members count before", 0, team1.Members().length);
    //team1.LoadFromObject(
        //);

    assertEquals("Team.Id", 1, team1.Id());
    assertEquals("Team.Name", "Management", team1.Name());
    assertEquals("Team.LeadFullName", "David Neigler", team1.LeadFullName());
    assertEquals("Team.LeadId", 2, team1.LeadId());
    assertEquals("Team.Members Count", 1, team1.Members().length);
    assertEquals("Team.Members[0]", 2, team1.Members()[0].Id());
    assertEquals("Team.Members[0].Projects[0]", 39, team1.Members()[0].Projects()[0].Id());
    console.log("TeamLoadWithZeroAllocations", team1.Members()[0].Projects()[0].Allocations()[2].Month());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations length s/b 5", 5, team1.Members()[0].Projects()[0].Allocations().length);
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[0]", 997, team1.Members()[0].Projects()[0].Allocations()[0].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[1]", 998, team1.Members()[0].Projects()[0].Allocations()[1].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[2]", 0, team1.Members()[0].Projects()[0].Allocations()[2].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[3]", 0, team1.Members()[0].Projects()[0].Allocations()[3].Id());
    assertEquals("Team.Members[0].Projects[0].MonthlyAllocations[4]", 0, team1.Members()[0].Projects()[0].Allocations()[4].Id());
    // can also check that this shows as default HasInsertion
};


ResourceAllocationTest.prototype.testHasInsertion = function () {
    var alloc = new monthlyAllocation({ "Month": "\/Date(1293858000000)\/", "Id": 0, "Allocation": 0.00000 });

    assertEquals("Alloc.Id", 0, alloc.Id());
    assertEquals("Alloc.Allocation", 0, alloc.Allocation());
    assertEquals("Alloc.Month", /Date(1293858000000)/, alloc.Month());
    assertTrue("Alloc.HasInsertion", alloc.HasInsertion());

    var alloc2 = new monthlyAllocation({ "Month": "\/Date(1296536400000)\/", "Id": 0, "Allocation": 0.00000 });

    assertEquals("Alloc.Id", 0, alloc2.Id());
    assertEquals("Alloc.Allocation", 0, alloc2.Allocation());
    assertEquals("Alloc.Month", /Date(1296536400000)/, alloc2.Month());
    assertTrue("Alloc.HasInsertion", alloc2.HasInsertion());

};

ResourceAllocationTest.prototype.testHasChanges = function () {
    var alloc = new monthlyAllocation({ "Month": "\/Date(1293858000000)\/", "Id": 1, "Allocation": 0.2500 });

    assertEquals("Alloc.Id", 1, alloc.Id());
    assertEquals("Alloc.Allocation", 0.25, alloc.Allocation());
    assertEquals("Alloc.Month", /Date(1293858000000)/, alloc.Month());
    // should not be insertion if Id starts > 0
    assertFalse("Alloc.HasInsertion", alloc.HasInsertion());
    assertFalse("Alloc.HasChanges", alloc.HasChanges());
    alloc.Allocation(.5);
    assertTrue("Alloc.HasChanges s/b true now", alloc.HasChanges());

    // test reversion
    alloc.Allocation(0.25);
    assertFalse("Alloc.HasChanges", alloc.HasChanges());

};