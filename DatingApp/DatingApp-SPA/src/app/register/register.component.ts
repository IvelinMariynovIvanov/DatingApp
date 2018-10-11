import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  // @Input() valuesfromHome: any;
  @Output() cancelRegister = new EventEmitter;
  // model: any = { };
  user: User;
  registerform: FormGroup;
  // all proprties become optional
  bsDatePickerConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private alertify: AlertifyService,
    private fb: FormBuilder, private route: Router) { }

  ngOnInit() {

    this.bsDatePickerConfig = {
      containerClass: 'theme-red'
    },

    this.createregisterform();

    // this.registerform = new FormGroup({
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmpassword: new FormControl('', Validators.required)
    // }, this.passwordMatchValidator);
  }

  createregisterform() {
    this.registerform = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmpassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmpassword').value ? null : {'mismatch': true};
  }

  register() {
    if (this.registerform.valid) {
      // create user from registerform values
      this.user = Object.assign({}, this.registerform.value);

        this.authService.register(this.user).subscribe(() => {
        this.alertify.success('register success');
    }, error => {
      this.alertify.error(error);
    }, () => {
      // on complete
      this.authService.login(this.user).subscribe(() => {
        this.route.navigate(['/members']);
      });
    });
    }
  // //  console.log(this.model);
  // this.authService.register(this.model).subscribe(() => {
  //   // console.log('register success');
  //   this.alertify.success('register success');
  // }, error => {
  //   this.alertify.error(error);
  //   // console.log('rigister error');
  // });

  }

  cancel() {
    this.cancelRegister.emit(false);
   //  console.log('canceled');
  }

}
