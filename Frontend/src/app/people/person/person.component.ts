import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Person, PersonWithOthers, Staff } from '../person';
import { PersonService } from '../person.service';
import { Role } from '../role';
import { OrgGroup } from '../groups/org-group';
import { MatDialog, MatSnackBar } from '@angular/material';
import { ConfirmDialogComponent } from '../../dialog/confirm-dialog/confirm-dialog.component';
import { NgModel } from '@angular/forms';
import { EmergencyContactExtended } from '../emergency-contact';
import { EmergencyContactComponent } from './emergency-contact/emergency-contact.component';
import { RoleComponent } from './role.component';
import { countries } from '../countries';
import { Observable } from 'rxjs/Observable';
import { map, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-person',
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.scss']
})
export class PersonComponent implements OnInit {
  public isNew: boolean;
  public filteredCountries: Observable<string[]>;
  public person: PersonWithOthers;
  public groups: OrgGroup[];
  public people: Person[];
  public peopleMap: { [key: string]: Person } = {};
  public newEmergencyContact = new EmergencyContactExtended();
  public newRole = new Role();
  @ViewChild('newEmergencyContactEl') newEmergencyContactEl: EmergencyContactComponent;
  @ViewChild('newRoleEl') newRoleEl: RoleComponent;
  @ViewChild('isStaff') isStaffElement: NgModel;
  @ViewChild('countriesControl') countriesControl: NgModel;

  constructor(private route: ActivatedRoute,
              private personService: PersonService,
              private router: Router,
              private dialog: MatDialog,
              private snackBar: MatSnackBar) {
    this.route.data.subscribe((value: {
      person: PersonWithOthers,
      groups: OrgGroup[],
      people: Person[]
    }) => {
      this.person = value.person;
      this.groups = value.groups;
      this.isNew = !this.person.id;
      this.newRole.personId = this.person.id;
      this.newEmergencyContact.personId = this.person.id;
      this.people = value.people.filter(person => person.id != value.person.id);
      this.peopleMap = this.people.reduce((map, currentValue) => {
        map[currentValue.id] = currentValue;
        return map;
      }, {});
      this.newEmergencyContact.order = this.person.emergencyContacts.length + 1;
    });
  }

  ngOnInit(): void {
    this.filteredCountries = this.countriesControl.valueChanges
      .pipe(map(value => countries.filter(country => this.startsWith(country, value || '')))
      );
  }
  startsWith(value: string, test: string) {
    return value.toLowerCase().indexOf(test.toLowerCase()) === 0;
  }

  async isStaffChanged(isStaff: boolean): Promise<void> {
    if (isStaff) {
      this.person.staff = new Staff();
      return;
    }
    //deleting?
    if (!this.isNew) {
      const dialogRef = this.dialog.open(ConfirmDialogComponent,
        {
          data: ConfirmDialogComponent.Options(`Deleting staff, data will be lost, this can not be undone`,
            'Delete',
            'Cancel')
        });
      let result = await dialogRef.afterClosed().toPromise();
      if (!result) {
        //roll back switch
        this.isStaffElement.control.setValue(true, {emitEvent: false});
        return;
      }
    }
    this.person.staff = null;
  }

  async save(): Promise<void> {
    let savedPerson = await this.personService.updatePerson(this.person);
    if (this.isNew) {
      this.router.navigate(['people', 'edit', savedPerson.id]);
    } else {
      this.router.navigate(['/people/list']);
    }
    this.snackBar.open(`${savedPerson.preferredName} Saved`, null, {duration: 2000});
  }

  async deletePerson() {
    let result = await ConfirmDialogComponent.OpenWait(this.dialog, `Delete person?`, 'Delete', 'Cancel');
    if (!result) return;
    await this.personService.deletePerson(this.person.id);
    this.router.navigate(['/people/list']);
    this.snackBar.open(`${this.person.preferredName} Deleted`, null, {duration: 2000});
  }

  async saveRole(role: Role, isNew = false): Promise<void> {
    role = await this.personService.updateRole(role);
    if (isNew) {
      this.person.roles = [...this.person.roles, role];
      this.newRole = new Role();
      this.newRole.personId = this.person.id;
      this.newRoleEl.form.resetForm();
      this.snackBar.open(`Role Added`, null, {duration: 2000});
    } else {
      this.snackBar.open(`Role Saved`, null, {duration: 2000});
    }
  }

  async deleteRole(role: Role) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent,
      {
        data: ConfirmDialogComponent.Options(`Delete role ${role.name}?`,
          'Delete',
          'Cancel')
      });
    let result = await dialogRef.afterClosed().toPromise();
    if (!result) return;
    await this.personService.deleteRole(role.id);
    this.person.roles = this.person.roles.filter(value => value.id != role.id);
    this.snackBar.open(`Role Deleted`, null, {duration: 2000});

  }

  async saveEmergencyContact(emergencyContact: EmergencyContactExtended, isNew = false) {
    emergencyContact = await this.personService.updateEmergencyContact(emergencyContact);
    if (isNew) {
      this.person.emergencyContacts = [...this.person.emergencyContacts, emergencyContact];
      this.newEmergencyContact = new EmergencyContactExtended();
      this.newEmergencyContact.personId = this.person.id;
      this.newEmergencyContact.order = this.person.emergencyContacts.length + 1;
      this.newEmergencyContactEl.form.resetForm();
      this.snackBar.open(`Emergency Contact Added`, null, {duration: 2000});
    } else {
      this.snackBar.open(`Emergency Contact Saved`, null, {duration: 2000});
    }
  }

  async deleteEmergencyContact(emergencyContact: EmergencyContactExtended) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent,
      {
        data: ConfirmDialogComponent.Options(`Delete Emergency Contact ${emergencyContact.contactPreferedName}?`,
          'Delete',
          'Cancel')
      });
    let result = await dialogRef.afterClosed().toPromise();
    if (!result) return;
    await this.personService.deleteEmergencyContact(emergencyContact.id);
    this.person.emergencyContacts = this.person.emergencyContacts.filter(value => value.id != emergencyContact.id);
    this.snackBar.open(`Emergency Contact Deleted`, null, {duration: 2000});
  }
}
