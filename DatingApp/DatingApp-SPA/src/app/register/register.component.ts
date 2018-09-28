import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  // @Input() valuesfromHome: any;
  @Output() cancelRegister = new EventEmitter;

  model: any = { };
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
  //  console.log(this.model);
  this.authService.register(this.model).subscribe(() => {
    console.log('register success');
  }, error => {
    console.log('rigister error');
  });
  }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('canceled');
  }

}
