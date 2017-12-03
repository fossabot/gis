﻿export class Role {

  constructor(public id?: string,
    public name?: string,
    public startDate?: Date,
    public active?: boolean,
    public endDate?: Date,
    public personId?: string) {
  }
}

export class RoleExtended extends Role {

  constructor(id: string,
    name: string,
    startDate: Date,
    active: boolean,
    endDate: Date,
    personId: string,
    public firstName: string,
    public lastName: string) {
    super(id, name, startDate, active, endDate, personId);
  }
}