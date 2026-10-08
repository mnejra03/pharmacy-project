import { Component, OnInit, OnDestroy } from '@angular/core';
import { AdminUserService, MyAppUser } from '../../../services/admin-user.service';
import { prepareFullUserObject } from '../../../helper/user-helper';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';
import {MyDialogConfirmComponent} from '../../shared/dialogs/my-dialog-confirm/my-dialog-confirm.component';
import {MatDialog} from '@angular/material/dialog';

@Component({
  selector: 'app-users',
  standalone: false,
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})

export class UsersComponent implements OnInit, OnDestroy {
  users: MyAppUser[] = [];


  createForm: FormGroup;
  showCreateForm = false;
  createError = '';


  filterUsername  = '';
  filterFirstName = '';
  filterLastName  = '';
  filterRole      = '';


  currentPage = 1;
  pageSize    = 10;
  totalCount  = 0;
  totalPages  = 0;

  isLoading = false;

  private filterSubject = new Subject<void>();
  private destroy$      = new Subject<void>();

  constructor(
    private userService: AdminUserService,
    private fb: FormBuilder,
    private dialog: MatDialog
  ) {
    this.createForm = this.fb.group({
      username:  ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName:  ['', [Validators.required, Validators.minLength(2)]],
      password:  ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {

    this.filterSubject.pipe(
      debounceTime(300),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.currentPage = 1;
      this.loadUsers();
    });
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUsers(): void {
    this.isLoading = true;

    this.userService.getUsersPaged(
      this.filterUsername,
      this.filterFirstName,
      this.filterLastName,
      this.filterRole,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: result => {
        this.users      = result.items;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.isLoading  = false;
      },
      error: err => {
        console.error('Error loading users:', err);
        this.isLoading = false;
      }
    });
  }

  applyFilter(): void {
    this.filterSubject.next();
  }

  resetFilter(): void {
    this.filterUsername  = '';
    this.filterFirstName = '';
    this.filterLastName  = '';
    this.filterRole      = '';
    this.currentPage     = 1;
    this.loadUsers();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadUsers();
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  onCreateUser(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const val = this.createForm.value;
    const dto = {
      username:     val.username,
      firstName:    val.firstName,
      lastName:     val.lastName,
      password:     val.password
    };

    this.userService.createUser(dto).subscribe({
      next: () => {
        this.createForm.reset();
        this.showCreateForm = false;
        this.createError    = '';
        this.loadUsers();
      },
      error: err => {
        this.createError = err.error?.Message || err.error || 'Error creating user.';
      }
    });
  }

  deleteUser(userId: number): void {
    const dialogRef = this.dialog.open(MyDialogConfirmComponent, {
      width: '400px',
      data: {
        title: 'Delete user',
        message: 'Are you sure you want to delete this user? This action cannot be undone.',
        confirmButtonText: 'Delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.userService.deleteUser(userId).subscribe({
          next: () => this.loadUsers(),
          error: err => console.error('Error deleting user:', err)
        });
      }
    });
  }

  toggleRole(user: any, selectedRole: 'isAdmin' | 'isPharmacist' | 'isCustomer'): void {
    user.isAdmin      = false;
    user.isPharmacist = false;
    user.isCustomer   = false;
    user[selectedRole] = true;

    const fullUser = prepareFullUserObject(user);
    this.userService.updateUser(fullUser).subscribe({
      next: () => {},
      error: err => console.error('Error updating role:', err)
    });
  }
}
